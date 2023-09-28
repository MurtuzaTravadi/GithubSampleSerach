using ContradoTest.Models;

namespace ContradoTest.ServiceLayer
{
    public interface IGitHubService
    {
        Task<UserAndRepositories> Search(string username);
    }
}
