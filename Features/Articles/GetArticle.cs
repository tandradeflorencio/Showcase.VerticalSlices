using Carter;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Showcase.VerticalSlice.Contracts;
using Showcase.VerticalSlice.Database;

namespace Showcase.VerticalSlice.Features.Articles
{
    public static class GetArticle
    {
        public class Query : IRequest<ArticleResponse>
        {
            public Guid Id { get; set; }
        }

        internal sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Query, ArticleResponse>
        {
            async Task<ArticleResponse> IRequestHandler<Query, ArticleResponse>.Handle(Query request, CancellationToken cancellationToken)
            {
                var article = await dbContext.
                    Article.
                    FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

                if (article is null)
                    return null!;

                return article.Adapt<ArticleResponse>();
            }
        }
    }

    public class GetArticleEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/articles/{id}", async (Guid id, ISender sender) =>
            {
                var query = new GetArticle.Query { Id = id };

                var result = await sender.Send(query);

                if (result is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(result);
            });
        }
    }
}