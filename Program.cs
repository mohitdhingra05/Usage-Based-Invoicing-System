using System;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace UsageBasedInvoicing
{
    class Program
    {
        private const decimal ApiTier1Rate = 0.01m;
        private const decimal ApiTier2Rate = 0.008m;
        private const int ApiTier1Threshold = 10000;
        private const decimal StorageRate = 0.25m;
        private const decimal ComputeRate = 0.05m;

        static void Main(string[] args)
        {
            string filePath = "C:\\Users\\dhing\\source\\repos\\Usage-Based Invoicing System\\usage-data.json";
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            string json = File.ReadAllText(filePath);
            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid JSON: " + ex.Message);
                return;
            }

            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                Console.WriteLine("Expected a JSON array.");
                return;
            }

            foreach (JsonElement item in doc.RootElement.EnumerateArray())
            {
                string customerId = GetString(item, "CustomerId").Trim();

                if (string.IsNullOrEmpty(customerId))
                {
                    Console.WriteLine("Skipped invalid entry: Missing or invalid fields for CustomerId: [unknown]");
                    continue;
                }

                if (!TryGetDecimal(item, "API_Calls", out decimal apiCalls) ||
                    !TryGetDecimal(item, "Storage_GB", out decimal storageGb) ||
                    !TryGetDecimal(item, "Compute_Minutes", out decimal computeMinutes))
                {
                    Console.WriteLine($"Skipped invalid entry: Missing or invalid fields for CustomerId: {customerId}");
                    continue;
                }

                decimal apiCost = CalculateApiCost(apiCalls);
                decimal storageCost = storageGb * StorageRate;
                decimal computeCost = computeMinutes * ComputeRate;
                decimal total = apiCost + storageCost + computeCost;

                Console.WriteLine($"Invoice for Customer: {customerId}");
                Console.WriteLine("-----------------------------");
                Console.WriteLine($"API Calls: {apiCalls:N0} calls -> {apiCost:C2}");
                Console.WriteLine($"Storage: {storageGb:0.##} GB -> {storageCost:C2}");
                Console.WriteLine($"Compute Time: {computeMinutes:N0} minutes -> {computeCost:C2}");
                Console.WriteLine("-----------------------------");
                Console.WriteLine($"Total Due: {total:C2}");
                Console.WriteLine();
            }

            doc.Dispose();
        }

        private static decimal CalculateApiCost(decimal apiCalls)
        {
            if (apiCalls <= ApiTier1Threshold)
            {
                return apiCalls * ApiTier1Rate;
            }

            decimal tier1Cost = ApiTier1Threshold * ApiTier1Rate;
            decimal tier2Calls = apiCalls - ApiTier1Threshold;
            decimal tier2Cost = tier2Calls * ApiTier2Rate;
            return tier1Cost + tier2Cost;
        }

        private static string GetString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out JsonElement child))
                return string.Empty;

            return child.ValueKind switch
            {
                JsonValueKind.String => child.GetString() ?? string.Empty,
                JsonValueKind.Number => child.GetRawText(),
                _ => string.Empty,
            };
        }

        private static bool TryGetDecimal(JsonElement element, string propertyName, out decimal value)
        {
            value = 0;
            if (!element.TryGetProperty(propertyName, out JsonElement child))
                return false;

            if (child.ValueKind == JsonValueKind.Number && child.TryGetDecimal(out value))
                return true;

            if (child.ValueKind == JsonValueKind.String)
            {
                string? text = child.GetString();
                if (!string.IsNullOrWhiteSpace(text) &&
                    decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
