# Telegram Bot Token & Chat ID

## 1. Get Bot Token

1. Search **`@BotFather`** in Telegram and send `/newbot`.
2. Choose a name and a username ending in `bot`.
3. Copy the token BotFather replies with.

## 2. Get Chat ID

1. **Personal chat:** Open your bot and tap **Start** (`/start`).  
   **Group / Channel:** Add the bot to the group (and send `/start` or `@your_bot`), or add it as **Admin** in a channel and post a message.
2. Open this URL in your browser (replace `<BOT_TOKEN>`):
   `https://api.telegram.org/bot<BOT_TOKEN>/getUpdates`
3. Copy the `"chat":{"id":...}` number (group/channel IDs start with `-` or `-100`).

## 3. Test & Add to Orbit

1. Test in your browser: `https://api.telegram.org/bot<BOT_TOKEN>/sendMessage?chat_id=<CHAT_ID>&text=Orbit+test` (`"ok":true` means it works).
2. Paste **Telegram Chat ID** and **Telegram Bot Token** into `/channels/new` (or an existing channel at `/channels/{id}`) in Orbit.
