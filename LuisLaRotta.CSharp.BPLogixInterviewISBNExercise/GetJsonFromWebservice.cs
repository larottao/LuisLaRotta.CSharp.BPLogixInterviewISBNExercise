using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LuisLaRotta.CSharp.BPLogixInterviewISBNExercise
{
    public class GetJsonFromWebservice
    {
        public async Task<Tuple<Boolean, string>> get(string apiUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        String contents = await response.Content.ReadAsStringAsync();

                        if (String.IsNullOrEmpty(contents) || contents.Equals("[]"))
                        {
                            return new Tuple<Boolean, String>(false, "Empty response from server");
                        }

                        return new Tuple<Boolean, String>(true, contents);
                    }
                    else
                    {
                        return new Tuple<Boolean, String>(false, response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Tuple<Boolean, String>(false, ex.Message);
            }
        }
    }
}