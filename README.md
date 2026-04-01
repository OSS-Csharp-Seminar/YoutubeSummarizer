# YoutubeSummarizer

Youtube summarizer is a simple, intuitive app to help you track your channels of interest, getting notifications when new videos are released, along with an AI-generated summary to help you decide whether the video is worth a watch or better off being skipped. Never again will you waste 15 minutes watching a video before realizing it's not your cup of tea.

## Features
- Subscribing to a channel
- Unsubscribing from a channel
- AI summarization of newly uploaded videos, based on your summary preferences
- Configuring summary preferences (how you want videos to be summarized for the given channel)
- Configuring a blacklist for topics/keywords
- Live notifications

## Additional technologies used
- PubSubHubub webhook used for receiving notifications of newly uploaded videos
- Ollama running in a docker container for the local LLM runtime, running a model yet to be specified (Probably Llama 3.1)
- youtube-transcript-api python library used for fetching transcripts of youtube videos to be summarized, executed via Python.NET.
- ngrok used for tunneling, allowing PubSubHubub to make webhook callbacks to a locally hosted API endpoint.
