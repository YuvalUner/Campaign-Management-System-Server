import json
import threading
import sys

import tweetnlp


class TextForClassification:
    def __init__(self, text):
        self.text = text
        self.topic = None
        self.sentiment = None
        self.hate = None

    def to_dict(self):
        return {
            'text': self.text,
            'topic': self.topic,
            'sentiment': self.sentiment,
            'hate': self.hate
        }


class ModelCollection:
    def __init__(self):
        self.models = {}

    def load_model(self, name, multi_label=True):
        if name not in self.models:
            if name == "topic_classification":
                self.models[name] = tweetnlp.load_model(name, multi_label=multi_label)
            else:
                self.models[name] = tweetnlp.load_model(name)
        return self.models[name]


def classify_topic(texts: [TextForClassification], model):
    for text in texts:
        text.topic = model.predict(text.text)['label']


def classify_sentiment(texts: [TextForClassification], model):
    for text in texts:
        text.sentiment = model.predict(text.text)['label']


def classify_hate(texts: [TextForClassification], model):
    for text in texts:
        text.hate = model.predict(text.text)['label']


if __name__ == '__main__':

    input_json_file = open(sys.argv[1], 'r')
    input_texts = json.load(input_json_file)
    input_json_file.close()

    articles = [TextForClassification(text) for text in input_texts['articles']]
    target_tweets = [TextForClassification(text) for text in input_texts['targetTweets']]
    tweets_about_target = [TextForClassification(text) for text in input_texts['tweetsAboutTarget']]
    model_collection = ModelCollection()

    # Load all the models in parallel
    threads = [
        threading.Thread(target=model_collection.load_model, args=('topic_classification', False)),
        threading.Thread(target=model_collection.load_model, args=('sentiment',)),
        threading.Thread(target=model_collection.load_model, args=('hate',))
    ]

    for thread in threads:
        thread.start()

    for thread in threads:
        thread.join()

    # Classify all the texts in parallel
    classification_threads = [
        threading.Thread(target=classify_topic, args=(articles, model_collection.models['topic_classification'])),
        threading.Thread(target=classify_topic, args=(target_tweets, model_collection.models['topic_classification'])),
        threading.Thread(target=classify_topic, args=(tweets_about_target, model_collection.models['topic_classification'])),
        threading.Thread(target=classify_sentiment, args=(articles, model_collection.models['sentiment'])),
        threading.Thread(target=classify_sentiment, args=(target_tweets, model_collection.models['sentiment'])),
        threading.Thread(target=classify_sentiment, args=(tweets_about_target, model_collection.models['sentiment'])),
        threading.Thread(target=classify_hate, args=(articles, model_collection.models['hate'])),
        threading.Thread(target=classify_hate, args=(target_tweets, model_collection.models['hate'])),
        threading.Thread(target=classify_hate, args=(tweets_about_target, model_collection.models['hate']))
    ]

    for thread in classification_threads:
        thread.start()

    for thread in classification_threads:
        thread.join()

    output_articles = {"articles": [article.to_dict() for article in articles]}
    output_target_tweets = {"target_tweets": [tweet.to_dict() for tweet in target_tweets]}
    output_tweets_about_target = {"tweets_about_target": [tweet.to_dict() for tweet in tweets_about_target]}
    combined_json = {**output_articles, **output_target_tweets, **output_tweets_about_target}
    print(json.dumps(combined_json))

    exit(0)
