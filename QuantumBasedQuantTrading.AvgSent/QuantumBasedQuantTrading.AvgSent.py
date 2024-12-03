import sys
import json
from textblob import TextBlob

def calculate_sentiment_normalized(text):
    if text is None or text.strip() == "":
        return 0.5
    analysis = TextBlob(text)
    sentiment_score = analysis.sentiment.polarity 
    normalized_score = (sentiment_score + 1) / 2 
    return round(normalized_score, 4)

def process_item(item):
    title = item.get('Title')
    description = item.get('Description')
    content = item.get('Content')
    published_at = item.get('Published_at')

    title_sentiment = calculate_sentiment_normalized(title)
    content_sentiment = calculate_sentiment_normalized(content)
    description_sentiment = calculate_sentiment_normalized(description)

    return {
        'Published_at': published_at,
        'TitleSentiment': title_sentiment,
        'ContentSentiment': content_sentiment,
        'DescriptionSentiment': description_sentiment
    }

def main():
    try:
        while True:
            input_data = sys.stdin.readline().strip()
            if not input_data:
                break

            item = json.loads(input_data)
            result = process_item(item)

            print(json.dumps(result))
            sys.stdout.flush()

    except Exception as e:
        print(json.dumps({"error": str(e)}), file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()
