namespace PasteBinDataBaseManager_Custom;

public class WebReader
{
    public static async Task<string[]> ReadFromUrl(string url)
    {
        List<string> result = new List<string>();

        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                // Read the response content as a string
                string content = await response.Content.ReadAsStringAsync();

                // Split the string into an array of lines
                string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                // Output the lines to the console
                foreach (string line in lines)
                {
                    result.Add(line);
                }

                return result.ToArray();
            }
        }
    }
}