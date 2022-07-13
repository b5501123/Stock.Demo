# Stock.Demo
因為本身對股票很有興趣，想要做一個股票即時的Notify，讓自己掌握第一手消息，並且分析出有意義的資料當作買點參考。
基於.net6 開發，結合Redis跟 MySQL去做資料處理 然後透過 telegram Bot去做Notify，
同時使用AWS的免費專案EC2及RDS MySQL，在雲端部屬，服務現在正在運行中


# 功能

- 營收爬蟲及分析，去分析這個月年成長率超過100%的公司
- 股票新聞爬蟲及分析，去公開資訊網，找出公布盈餘及股利相關訊息
- 每日股票收盤爬蟲分析，去找出股票的跳漲跳空股，以及成交量成長股
- 每日股票法人爬蟲分析，找出外資及投信持續買超的股票
- 盈餘股票通知，分析EPS及本益比有沒有溢價
- 股票及時漲停報及快速上漲通知，這個要配合卷商的SDK，目前Demo功能不會提供

# 示意圖

- 漲停報

![image](https://user-images.githubusercontent.com/57789269/177086817-30047bfb-0e4d-4c27-8ea9-e71c2750807a.png)

- 自結損益新聞

![image](https://user-images.githubusercontent.com/57789269/177086906-a8ba1364-ba2e-420c-a450-f08fc06856a1.png)

- 營收成長快報

![image](https://user-images.githubusercontent.com/57789269/177087083-7a572560-7987-483a-a021-455365f01487.png)


