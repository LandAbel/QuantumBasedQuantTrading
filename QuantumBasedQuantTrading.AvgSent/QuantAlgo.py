from multiprocessing import Value
import sys
import json
from threading import Thread
from xmlrpc.client import _datetime
import pandas as pd
import yfinance as yf
from sklearn.model_selection import train_test_split
from tensorflow import keras
from tensorflow.keras import layers
import tensorflow as tf
from qiskit_ibm_runtime import QiskitRuntimeService
from qiskit import QuantumCircuit, transpile
from qiskit_aer import AerSimulator
import numpy as np
import os
from datetime import datetime
import warnings
import sys
from tensorflow.keras.utils import register_keras_serializable
import io
from qiskit.circuit import Parameter
import random
from scipy.optimize import minimize

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')

try:
    warnings.filterwarnings("ignore", category=UserWarning)
    warnings.filterwarnings("ignore", category=DeprecationWarning)
except Exception as e:
    sys.exit(1)

api_key = os.getenv("IBM_QUANTUM_API_KEY")
if api_key:
    QiskitRuntimeService.save_account(channel="ibm_quantum", token=api_key, overwrite=True)
    provider = QiskitRuntimeService()
else:
    print("Warning: IBM Quantum API key not found in environment variables. Fallback to local Aer simulator will be used.")



def simulate_quantum_circuit(circuit, parameter_values):
    try:
        provider = QiskitRuntimeService(channel="ibm_quantum")
        backend = provider.backend('ibm_kyiv')
        status = backend.status()
        if (status.operational or status.pending_jobs > 50):
            backend = AerSimulator()
        else: 
            backend = provider.backend('ibm_kyiv')
    except Exception as e:
        print(f"Falling back to Aer simulator due to error: {e}")
        backend = AerSimulator()

    bound_circuit = circuit.assign_parameters(parameter_values)
    new_circ = transpile(bound_circuit, backend)
    job = backend.run(new_circ,shots=1000)
    result = job.result()
    counts = result.get_counts()
    prob_0 = counts.get("0", 0) / sum(counts.values())
    return tf.constant(prob_0, shape=(1, 1))

def cost_function(params):
    param_dict = {theta[i]: params[i] for i in range(4)}
    param_dict.update({phi[i]: params[i + 4] for i in range(4)})
    param_dict.update({lam[i]: params[i + 8] for i in range(6)})

    quantum_output = simulate_quantum_circuit(qc, param_dict)
    loss = tf.reduce_mean((quantum_output - 0.5)**2)
    return loss.numpy()


def serialize_circuit(circuit):
    return {
        "num_qubits": circuit.num_qubits,
        "operations": [
            (gate.name, [circuit.find_bit(q).index for q in qargs])
            for gate, qargs, _ in circuit.data
        ]
    }

def deserialize_circuit(data):
    from qiskit import QuantumCircuit
    circuit = QuantumCircuit(data["num_qubits"])
    for gate_name, qubits in data["operations"]:
        getattr(circuit, gate_name)(*qubits)
    return circuit

@register_keras_serializable(package="Custom")
class QuantumInspiredLayer(keras.layers.Layer):
    def __init__(self, circuit, params):
            super().__init__()
            self.circuit = circuit
            self.params = params


    def call(self, inputs):
        quantum_output = simulate_quantum_circuit(self.circuit, self.params)
        return inputs * quantum_output

    def compute_output_shape(self, input_shape):
        return input_shape
    def get_config(self):
        config = super().get_config()
        config.update({
            "circuit_data": serialize_circuit(self.circuit)
        })
        return config

    @classmethod
    def from_config(cls, config):
        circuit = deserialize_circuit(config["circuit_data"])
        return cls(circuit, **config)


class PositionalEncoding(layers.Layer):
    def __init__(self, sequence_length, d_model):
        super().__init__()
        self.pos_encoding = self.positional_encoding(sequence_length, d_model)

    def positional_encoding(self, seq_len, d_model):
        pos = tf.cast(tf.range(seq_len)[:, tf.newaxis], dtype=tf.float32)
        i = tf.cast(tf.range(d_model)[tf.newaxis, :], dtype=tf.float32)
        angle_rates = 1 / tf.pow(10000.0, (2 * (i // 2)) / tf.cast(d_model, tf.float32))
        angle_rads = pos * angle_rates
        sines = tf.sin(angle_rads[:, 0::2])
        cosines = tf.cos(angle_rads[:, 1::2])
        return tf.cast(tf.concat([sines, cosines], axis=-1), dtype=tf.float32)

    def call(self, inputs):
        return inputs + self.pos_encoding

def transformer_block(inputs, num_heads, ff_dim, dropout_rate=0.1):
    attention_output = layers.MultiHeadAttention(num_heads=num_heads, key_dim=ff_dim)(inputs, inputs)
    attention_output = layers.Dropout(dropout_rate)(attention_output)
    attention_output = layers.LayerNormalization(epsilon=1e-6)(inputs + attention_output)

    ff_output = layers.Dense(ff_dim, activation="relu")(attention_output)
    ff_output = layers.Dense(inputs.shape[-1])(ff_output)
    ff_output = layers.Dropout(dropout_rate)(ff_output)
    return layers.LayerNormalization(epsilon=1e-6)(attention_output + ff_output)


if not sys.stdin.isatty():  # If input is piped
    input_data = sys.stdin.read()
else:  # Otherwise, read from command-line argument
    try:
        input_data = sys.argv[1]
    except IndexError:
        print("Error: No input JSON provided.", file=sys.stderr)
        sys.exit(1)

try:
    args = json.loads(input_data)
    df_sentiment = pd.DataFrame(args['sentiment_data'])
    symbol = args['symbol']
    start_date = datetime.strptime(args['start_date'], '%Y-%m-%d').replace(hour=0, minute=0, second=0)
    end_date = datetime.strptime(args['end_date'], '%Y-%m-%d').replace(hour=0, minute=0, second=0)
    title_sentiment = float(args['titleSent'])
    content_sentiment = float(args['contSent'])
    desc_sentiment = float(args['descSent'])
    open_price = float(args['openPrice'])
    current_high = float(args['currentHigh'])
    current_low = float(args['currentLow'])
    volume=float(args['volume'])
except Exception as e:
    print(f"Error parsing input JSON: {e}", file=sys.stderr)
    sys.exit(1)

try:
    df_stock = yf.download(symbol, start=start_date, end=end_date).reset_index()
    Numerical_columnsToDict = ['Open', 'Low', 'High', 'Volume', 'Close']
    train_min = df_stock[Numerical_columnsToDict].min().to_dict()
    train_max = df_stock[Numerical_columnsToDict].max().to_dict()
except Exception as e:
    print(f"Error downloading stock data: {e}", file=sys.stderr)
    sys.exit(1)

try:
    df_sentiment['date'] = pd.to_datetime(df_sentiment['date'])

    df = pd.merge(df_sentiment, df_stock, left_on='date', right_on='Date', how='inner').dropna()

    numerical_cols = ['title_sentiment', 'content_sentiment', 'description_sentiment', 'Open', 'High', 'Low', 'Close', 'Adj Close']
    df[numerical_cols] = (df[numerical_cols] - df[numerical_cols].min()) / (df[numerical_cols].max() - df[numerical_cols].min())
    df['Volume'] = (df['Volume'] - df['Volume'].min()) / (df['Volume'].max() - df['Volume'].min())#Seperate normalization beacuse it would ruin the true contrast

    X = df[['title_sentiment', 'content_sentiment', 'description_sentiment', 'Open', 'Low', 'High', 'Volume']]
    X = np.expand_dims(X, axis=1)
    X = np.repeat(X, 10, axis=1)
    X = X.astype(np.float32)

    y = df['Close'].astype(np.float32)

    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.3, random_state=42)
    X_test, X_val, y_test, y_val = train_test_split(X_test, y_test, test_size=0.5, random_state=42)
except Exception as e:
    print(f"Error processing data: {e}", file=sys.stderr)
    sys.exit(1)


theta = [Parameter(f'theta_{i}') for i in range(4)]
phi = [Parameter(f'phi_{i}') for i in range(4)]
lam = [Parameter(f'lambda_{i}') for i in range(6)]

qc = QuantumCircuit(4)

qc.ry(theta[0], 0)
qc.ry(theta[1], 1)
qc.ry(theta[2], 2)
qc.ry(theta[3], 3)

qc.cx(0, 1)
qc.cx(2, 3)

qc.rz(phi[0], 0)
qc.ry(phi[1], 1)
qc.rz(phi[2], 2)
qc.rx(phi[3], 3)

qc.cx(0, 2)
qc.cx(1, 3)

qc.ry(lam[0], 0)
qc.rz(lam[1], 1)
qc.rx(lam[2], 2)
qc.rz(lam[3], 3)

qc.cx(0, 1)
qc.cx(1, 2)
qc.cx(2, 3)
qc.cx(3, 0)

qc.ry(lam[4], 0)
qc.rx(lam[5], 1)
qc.rz(lam[0], 2)
qc.ry(lam[1], 3)

qc.measure_all()

initial_params = np.random.uniform(0, 2 * np.pi, size=14)
result = minimize(cost_function, initial_params, method='BFGS')
optimized_params = result.x

parameter_values = {theta[i]: optimized_params[i] for i in range(4)}
parameter_values.update({phi[i]: optimized_params[i + 4] for i in range(4)})
parameter_values.update({lam[i]: optimized_params[i + 8] for i in range(6)})

try:
    def create_model(input_shape, circuit, sequence_length=10, d_model=64, num_heads=4, ff_dim=128):
        inputs = keras.Input(shape=(sequence_length, input_shape), name="model_input")
        x = layers.Dense(d_model)(inputs) 
        x = PositionalEncoding(sequence_length, d_model)(x)

        for _ in range(2):
            x = transformer_block(x, num_heads=num_heads, ff_dim=ff_dim)

        x = layers.Flatten()(x)
        x = layers.Dense(32, activation='relu')(x)
        x = layers.Dropout(0.2)(x)
        x = layers.Dense(64, activation='relu')(x)
        x = layers.Dropout(0.2)(x)

        quantum_layer = QuantumInspiredLayer(circuit, parameter_values )(x)
        output = layers.Dense(1, activation='linear', name="model_output")(quantum_layer)
        return keras.Model(inputs, output)


    model = create_model(input_shape=7, circuit=qc, sequence_length=10, d_model=64, num_heads=4, ff_dim=128)
    model.compile(optimizer='adam', loss='mse', metrics=['mae'])
except Exception as e:
    print(f"Error creating or compiling model: {e} {X.dtype} {y.dtype}", file=sys.stderr)
    sys.exit(1)

try:
    callbacks = [tf.keras.callbacks.EarlyStopping(monitor='val_loss', patience=10)]
    history = model.fit(X_train, y_train, validation_data=(X_val, y_val), epochs=1000, batch_size=32, callbacks=callbacks)
except Exception as e:
    print(f"Error training model: {e}", file=sys.stderr)
    sys.exit(1)

try:
    # Evaluate the model
    train_loss, train_mae = model.evaluate(X_train, y_train)
    val_loss, val_mae = model.evaluate(X_val, y_val)
    test_loss, test_mae = model.evaluate(X_test, y_test)
except Exception as e:
    print(f"Error evaluating model: {e}", file=sys.stderr)
    sys.exit(1)

#try:
#    save_model = "model.keras"
#    model.save(save_model)

#except Exception as e:
#    sys.exit(1)

try:
    data = [title_sentiment, content_sentiment, desc_sentiment, open_price, current_low, current_high, volume]
    normalized_data = data[:3]
    columns_to_normalize = ['Open', 'Low', 'High', 'Volume']
    for i, col in enumerate(columns_to_normalize):
        value = data[i + 3]
        normalized_value = (value - train_min[col]) / (train_max[col] - train_min[col])
        normalized_data.append(normalized_value)
    
    input_data = np.array(normalized_data).reshape(1, 1, -1)
    input_data = np.repeat(input_data, 10, axis=1)

    predicted_y_train = model.predict(input_data)
    predicted_close=predicted_y_train[0] * (train_max['Close'] - train_min['Close']) + train_min['Close']
    pred_value=float(predicted_close[0])

except Exception as e:
    sys.exit(1)
os.system('cls' if os.name == 'nt' else 'clear')
output = {
    "train_mae": train_mae,
    "val_mae": val_mae,
    "test_mae": test_mae,
    "predicted_value": pred_value,
}

print(json.dumps(output))