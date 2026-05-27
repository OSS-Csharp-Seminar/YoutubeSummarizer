# YoutubeSummarizer

Youtube summarizer is a simple, intuitive app to help you track your channels of interest, getting notifications when new videos are released, along with an AI-generated summary to help you decide whether the video is worth a watch or better off being skipped. Never again will you waste 15 minutes watching a video before realizing it's not your cup of tea.

## Features
- Subscribing/unsubscribing to YouTube channels
- AI summarization of newly uploaded videos, with configurable summarization styles per channel (Brief, Detailed, Scientific, Layman)
- Keyword blacklist to filter out unwanted topics
- Live push notifications via SignalR
- User authentication with JWT (httpOnly cookies)
- Admin panel (user management, banning, mock webhook testing, global notifications)
- Account deletion

## Tech stack
- **Backend:** ASP.NET Core Web API (.NET 10), Clean Architecture
- **Frontend:** Blazor Interactive WebAssembly
- **Database:** SQL Server (Docker)
- **Validation:** FluentValidation with a global action filter
- **Real-time:** SignalR

## External services
- **PubSubHubbub** webhook for receiving notifications of newly uploaded videos
- **OpenRouter** as the primary LLM provider for summarization, with **Ollama** (Docker) as a local fallback
- **YouTube Transcript API** (external HTTP service) for fetching video transcripts
- **ngrok** (Docker) for tunneling PubSubHubbub webhook callbacks to a locally hosted API
