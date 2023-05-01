using FrontEndAPI.Models;


namespace PublisherMiniFrontEnd.Services
{
    public class ContractedAuthorsService
    {

        private readonly HttpClient _httpClient;
        private readonly string _baseAddress;

        public ContractedAuthorsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseAddress = $"{_httpClient.BaseAddress.ToString()}authors";
        }


        //get author list all  authors
        public async Task<List<AuthorName>> ListAllAuthorsAsync()
        {

            var response=  _httpClient.GetAsync(_baseAddress).Result;
            var authors = await response.Content.ReadFromJsonAsync<List<AuthorName>>();
            return authors;
        }
        //get author list subset by first

        public async Task<List<AuthorName>> GetAuthorsByFirstNameAsync(string firstname)
        {
            string url=$"{_baseAddress}/firstname/{firstname}";
            var response = _httpClient.GetAsync(url).Result;
            var authors = await response.Content.ReadFromJsonAsync<List<AuthorName>>();
            return authors;
        }

        //get author list subset by last
        public async Task<List<AuthorName>> GetAuthorsByLastNameAsync(string lastname)
        {
            string url = $"{_baseAddress}/lastname/{lastname}";
            var response = _httpClient.GetAsync(url).Result;
            var authors = await response.Content.ReadFromJsonAsync<List<AuthorName>>();
            return authors;
        }


    }
}
