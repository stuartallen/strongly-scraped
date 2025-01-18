# Scraper API

- Scrapes data from iron podium
- Save to database
- Create vector store from each page/event description
- Serve endpoint for normal filtering and vector search

## TODO:

- [x] Scrape data from iron podium
- [x] Dockerize
- [ ] Save to database
- [ ] Create vector store from each page/event description
- [ ] Serve endpoint for normal filtering
- [ ] Serve endpoint for vector search
- [ ] Deploy to Fly.io

## Run without docker

```
dotnet run
```

## Run with docker

```
docker build -t scraper .
docker run -d -p 5250:5250 scraper
```
