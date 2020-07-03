# Adventure Works Thumbs

> A simple console app that will convert all of the distinct Product Thumbnails into their proper pictures - from the SQL DB to your local file system, so you can put them anywhere.

This little app was built in conjunction with a Proof-of-Concept and blog post I did surrounding the new Preview version of custom verticals for SharePoint Graph Connectors. In the post, I built a serverless Azure SQL db and treated it like an "on-prem" SQL database. I loaded the database with AdventureWorks data, which I called HomolWorks (naturally 😜). Then I setup a Power BI gateway on an Azure VM, which connected to the database. This gateway is then used by the Graph Connector for SQL Server. My queries centered around pulling and indexing the products in the Adventure Works data. In the Products table, there is a ThumbNailPhoto field and a ThumbnailPhotoFileName field. Since this was just a POC, rather than build an ASP.Net service that pulls the data for each filename and returns the bit stream via URL, it seemed easier for me to just get them loaded up statically once and forever. Hence this little app. If it's useful to you, have at it.

Here's a sample of a row, formatted to JSON:

```json
{
  "@odata.context": "http://localhost:35027/odata/AdventureWorks/$metadata#Products",
  "value": [
    {
      "ProductID": 714,
      "Name": "Long-Sleeve Logo Jersey, M",
      "ProductNumber": "LJ-0192-M",
      "Color": "Multi",
      "StandardCost": 38.4923,
      "ListPrice": 49.99,
      "Size": "M",
      "Weight": null,
      "ProductCategoryID": 25,
      "ProductModelID": 11,
      "SellStartDate": "2001-07-01",
      "SellEndDate": null,
      "DiscontinuedDate": null,
      "ThumbNailPhoto": "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",
      "ThumbnailPhotoFileName": "awc_jersey_male_small.gif",
      "rowguid": "6a290063-a0cf-432a-8110-2ea0fda14308",
      "ModifiedDate": "2004-03-11"
    }
  ]
}
```
