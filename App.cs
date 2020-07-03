using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text;
using System;
using System.IO;

namespace AdventureWorksThumbs
{
    public class App
    {
        private readonly IConfigurationRoot _config;
        private readonly ILogger<App> _logger;

        public App(IConfigurationRoot config, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<App>();
            _config = config;
        }

        public async Task Run()
        {
            // Print connection string to demonstrate configuration object is populated
            Console.WriteLine(_config.GetConnectionString("DataConnection"));

            // 📁 Check if the thumbnails folder exists and, if not, create it
            var thumbnailsFolder = "thumbnails/";
            if (!Directory.Exists(thumbnailsFolder))
            {
                Directory.CreateDirectory(thumbnailsFolder);
            }

            Console.WriteLine("Connecting to AdventureWorks DB...");

            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("DataConnection")))
                {
                    StringBuilder distinctThumbnailsStatement = new StringBuilder();

                    distinctThumbnailsStatement.AppendLine("SELECT DISTINCT [ThumbNailPhoto], [ThumbnailPhotoFileName]");
                    distinctThumbnailsStatement.AppendLine("FROM [SalesLT].[Product]");

                    await conn.OpenAsync();

                    using (SqlCommand distinctThumbnailsGet = new SqlCommand(distinctThumbnailsStatement.ToString(), conn))
                    {
                        using (SqlDataReader distinctThumbnails = await distinctThumbnailsGet.ExecuteReaderAsync())
                        {
                            while (distinctThumbnails.Read())
                            {
                                //Get the file name
                                var thumbnailFileName = distinctThumbnails.GetString(1);
                                Console.WriteLine($"{thumbnailFileName}");

                                //Get the thumbnail byte array length
                                var thumbnailByteLength = (int)distinctThumbnails.GetBytes(0, 0, null, 0, 0);
                                // initialize the thumbnail byte array
                                byte[] thumbnailBytes = new byte[thumbnailByteLength];
                                // fill it
                                distinctThumbnails.GetBytes(0, 0, thumbnailBytes, 0, thumbnailByteLength);

                                // 🗒 Save the thumbnail to a file (also in a thumbnails folder)
                                await File.WriteAllBytesAsync(Path.Combine(thumbnailsFolder, thumbnailFileName), thumbnailBytes);
                            }
                        }
                    }
                }
            }
            catch (SqlException sex)
            {
                Console.Beep();
                Console.WriteLine(sex.ToString());
            }
        }
    }
}