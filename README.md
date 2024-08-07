
# API Basketball Historical Player Stat Tracker

This is a final project for TrueCoders. It displays historical data for various NBA teams. It allows users to put in the year they would like to view and what team they would like to see. I provided a link that takes you to a wiki page that displays each NBA team's abbreviation. It uses an API called "NBA-API" that can be found on Rapidapi.com

## Environment Variables

To run this project, you will need to add the following environment variables to your appsettings.json file

`API_KEY`

`Host Information`
```json
    {
      "ApiSettings": {
        "RapidApiKey": "your-rapidapi-key",
        "RapidApiHost": "api-nba-v1.p.rapidapi.com"
      }
    }
    ```

## Dependencies

This uses the Newtonsoft.Json Nuget Package


## API Reference

#### Get all items

```http
  GET /team/statistics 
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `season` | `int` | **Required** The season year to fetch |

#### Get item

```http
  GET /api/items/${id}
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `team_id`      | `int` | **Required**. Id of a team to fetch |


It takes two parameters and returns the season historical data for the team of your choosing.

