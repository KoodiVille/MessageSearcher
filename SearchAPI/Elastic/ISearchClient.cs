using SearchAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SearchAPI.InitScrollRequest.Types;

namespace SearchAPI.Elastic
{
    public interface ISearchClient
    {
        Task<IEnumerable<Message>> Search(string query, int from, int size = 20);
        Task<ScrollResult> InitScroll(int size, string timestamp, Direction direction);
        Task<ScrollResult> Scroll(string scrollId);

    }
}
