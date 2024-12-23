﻿using InfluxDB.Client.Writes;
using InfluxDB.Client;
using RasperryInflux.Entities;
using System;

namespace RasperryInflux.Data.InfluxDB
{
    public class InfluxRepository : IInfluxRepository
    {

        const string hostUrl = "http://brian.local:8086";
        const string? database = "KrillzOfDoom";
        const string? authToken = "OlNg2BYK4DZDmdU7s1REd2lNGnhUS42a5JOJmDv8chyJ3hKbL52_KtZIFM3So-aFynzTB5M1hCRrKeV92doFNg==";


        public async Task WriteTelemetry(Telemetry telemetry)
        {
            using var client = new InfluxDBClient(hostUrl,  authToken);

            PointData point = PointData.Measurement("Telemetry")
                .Field("Temperature", telemetry.temperature)
                .Field("Humidity", telemetry.humidity);
            try
            {
                await client.GetWriteApiAsync().WritePointAsync(point: point, bucket: database, org : "EUC");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public async Task<List<Telemetry>> QuereDbAsync(string option)
        {
            string quere = option switch
            {
                "All" => "from(bucket: \"KrillzOfDoom\") \r\n|> range(start: -24h)\r\n|> filter(fn: (r) => r[\"_measurement\"] ==\"Telemetry\")\r\n|> filter(fn: (r) => r[\"_field\"] == \"Humidity\" or r[\"_field\"] == \"Temperature\")\r\n|> pivot (rowKey: [\"_time\"], columnKey: [\"_field\"], valueColumn: \"_value\")"
,
                "LastHour" => "SELECT * FROM 'TemperatureData' WHERE time >= now() - interval '1 hour' AND 'Humidity' IS NOT NULL OR 'Temperature' IS NOT NULL",
                "Today" => "SELECT * FROM 'TemperatureData' WHERE time >= today() AND 'Humidity' IS NOT NULL OR 'Temperature' IS NOT NULL"
            };

            List<Telemetry> list = new List<Telemetry>();

            using var client = new InfluxDBClient(hostUrl, authToken);

            foreach (var table in await client.GetQueryApi().QueryAsync(quere, "EUC"))
            {
                foreach (var records in table.Records)
                {
                    list.Add(new Telemetry()
                    {
                        humidity = Convert.ToDouble(records.GetValueByKey("Humidity")),
                        temperature = Convert.ToDouble(records.GetValueByKey("Temperature")),
                        time = DateTime.Parse(records.GetValueByKey("_time").ToString()).ToLocalTime()
                    });
                }
            }

            list = list.GroupBy(x => x.time.ToString("yy/MM/dd HH/mm/ss"))
                .Select(x =>
                {
                    Telemetry average = x.First();
                    average.humidity = x.Average(y => y.humidity);
                    average.temperature = x.Average(y => y.temperature);
                    average.time = new DateTime((long)x.Average(item => item.time.Ticks));
                    return average;
                }).OrderByDescending(x => x.time).Take(10).ToList();
            return list;
        }
    }
}
