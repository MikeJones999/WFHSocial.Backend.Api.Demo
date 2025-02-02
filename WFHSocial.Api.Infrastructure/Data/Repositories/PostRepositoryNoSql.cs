using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using WFHSocial.Api.Domain.Posts.DTOs.PostModels;
using WHFSocial.Api.Domain.Posts.Interfaces.Repositories;
using WHFSocial.Api.Domain.Posts.Models;

namespace WFHSocial.Api.Infrastructure.Data.Repositories
{
    public class PostRepositoryNoSql : IPostRepositoryNoSql
    {
        private readonly ILogger<PostRepositoryNoSql> _logger;
        private readonly Database _cosmosDatabase;

        public PostRepositoryNoSql(ILogger<PostRepositoryNoSql> logger, Database cosmosDatabase)
        {
            _logger = logger;
            _cosmosDatabase = cosmosDatabase;
        }

        public async Task<List<Post>> GetUsersPersonalPostsAsync(Guid userId)
        {
            List<Post> posts = new List<Post>();

            Container cosmosContainer = _cosmosDatabase.GetContainer(ContainerConstants.Posts);
            FeedIterator<Post> feedIterator = cosmosContainer.GetItemQueryIterator<Post>(
                queryText: $"SELECT * FROM {ContainerConstants.Posts} WHERE {ContainerConstants.Posts}.id = '{userId}'"
                );  

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<Post> currentResultSet = await feedIterator.ReadNextAsync();
                posts.AddRange(currentResultSet);
            }

            return posts;
        }

        public async Task<Post?> GetUsersPersonalPostByIdAsync(Guid userId, Guid postId)
        {
            Post? post = null;
            Container cosmosContainer = _cosmosDatabase.GetContainer(ContainerConstants.Posts);
            FeedIterator<Post> feedIterator = cosmosContainer.GetItemQueryIterator<Post>(
                queryText: $"SELECT * FROM {ContainerConstants.Posts} WHERE {ContainerConstants.Posts}.id = '{postId}'"
                );  

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<Post> currentResultSet = await feedIterator.ReadNextAsync();
                post = currentResultSet.FirstOrDefault();
            }

            return post;
        }

        public async Task<Guid?> CreateNewPostAsync(Post post)
        {
            Guid postId = Guid.NewGuid();
            post.Id = postId.ToString();
            Container cosmosContainer = _cosmosDatabase.GetContainer(ContainerConstants.Posts);
            ItemResponse<Post> response = await cosmosContainer.CreateItemAsync(post, partitionKey: new PartitionKey(postId.ToString()));
            return response.StatusCode == System.Net.HttpStatusCode.Created ? postId : null;
        }

        public async Task<(List<Post>? posts, int total)> GetUsersPersonalPostsByUserIdAsync(Guid userId, GetPostListFilter filter)
        {
            Container cosmosContainer = _cosmosDatabase.GetContainer(ContainerConstants.Posts);
            int count = await GetPostCountAsync(userId);
            string orderValue = filter.IsDescendingByDate ? "DESC" : "ASC";      
            FeedIterator<Post> feedIterator = cosmosContainer.GetItemQueryIterator<Post>(
            queryText: $"SELECT * " +
            $"FROM {ContainerConstants.Posts}" +
            $" WHERE {ContainerConstants.Posts}.AuthorId = '{userId}' " +
            $"ORDER BY {ContainerConstants.Posts}.PostDateUtc {orderValue} " +
            $"OFFSET {filter.Getskip()} LIMIT {filter.PageSize}"
            );  
            List<Post> posts = new List<Post>();
            
            while (feedIterator.HasMoreResults)
            {
                FeedResponse<Post> currentResultSet = await feedIterator.ReadNextAsync();   
                foreach (Post item in currentResultSet)
                {
                    posts.Add(new Post
                    {
                        Id = item.Id,
                        AuthorId = item.AuthorId,
                        PostDateUtc = item.PostDateUtc,
                        PostContent = item.PostContent,    
                        Comments = item.Comments,
                        PostType = item.PostType
                    });
                }                
            }

            return (posts, count);
        }

        public async Task<int> GetPostCountAsync(Guid userId)
        {
            int count = 0;
            Container cosmosContainer = _cosmosDatabase.GetContainer(ContainerConstants.Posts);           

            FeedIterator<dynamic> countIterator = cosmosContainer.GetItemQueryIterator<dynamic>(
                queryText: $"SELECT VALUE COUNT(1)" +
                $" FROM {ContainerConstants.Posts}" +
                $" WHERE {ContainerConstants.Posts}.AuthorId = '{userId}'"
            );

            while (countIterator.HasMoreResults)
            {
                FeedResponse<dynamic> response = await countIterator.ReadNextAsync();                
                dynamic? stringNumber = response.Resource.FirstOrDefault();             
                count += ParseDynamicObjectToNumber(stringNumber);                     
            }
            return count;
        }

        private static long ParseDynamicObjectToNumber(object obj)
        {   
            if(obj == null || obj.ToString() == null)
            {
                return 0;
            }
            return long.Parse(obj.ToString()!);     
        }

        public Task<(List<Post>? posts, int total)> GetPostsByUsersByListOfIdsAsync(List<Guid> authorIds, GetPostListFilter filters)
        {
            List<Post>? posts = null;

            return Task.FromResult((posts, 0)); //Not implemented
        }
    }
}
