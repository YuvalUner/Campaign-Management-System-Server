import json
import sys

import snscrape.modules.twitter as sntwitter
import datetime


def scrape(search_query, max_tweets=100):
    # Created a list to append all tweet attributes(data)
    attributes_container = []

    # Using TwitterSearchScraper to scrape data and append tweets to list
    for i, tweet in enumerate(sntwitter.TwitterSearchScraper(query=search_query).get_items()):
        if i > max_tweets - 1:
            break
        attributes_container.append(tweet.rawContent)

    # Creating a dataframe from the tweets list above
    return attributes_container


if __name__ == '__main__':
    opponent_name = sys.argv[1]

    opponent_twitter_handle = sys.argv[2]
    opponent_twitter_handle = opponent_twitter_handle.replace('@', '')

    date_today = datetime.datetime.now().date()
    max_backwards_days = 30

    if len(sys.argv) > 3:
        max_backwards_days = int(sys.argv[3])

    date_backwards = date_today - datetime.timedelta(days=max_backwards_days)
    date_forwards = date_today + datetime.timedelta(days=1)

    query = f'from:{opponent_twitter_handle} since:{date_backwards} until:{date_forwards}'
    targeted_opponent_tweets = scrape(query)

    query = f'{opponent_name} since:{date_backwards} until:{date_forwards} filter:replies'
    tweets_about_opponent = scrape(query)

    combined_output = {'target_tweets': targeted_opponent_tweets, 'tweets_about_target': tweets_about_opponent}
    print(json.dumps(combined_output))

