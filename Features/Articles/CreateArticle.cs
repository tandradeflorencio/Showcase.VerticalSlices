using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Showcase.VerticalSlice.Contracts;
using Showcase.VerticalSlice.Database;
using Showcase.VerticalSlice.Entities;

namespace Showcase.VerticalSlice.Features.Articles
{
    public static class CreateArticle
    {
        public class Command() : IRequest<Guid>
        {
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public List<string> Tags { get; set; } = [];
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.Title).NotEmpty();
                RuleFor(c => c.Content).NotEmpty();
            }
        }

        internal sealed class Handler(ApplicationDbContext dbContext, IValidator<Command> validator) : IRequestHandler<Command, Guid>
        {
            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);

                if (!validationResult.IsValid)
                {
                }

                var article = new Article
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Content = request.Content,
                    Tags = request.Tags,
                    CreatedOnUtc = DateTime.UtcNow,
                };

                dbContext.Add(article);

                await dbContext.SaveChangesAsync(cancellationToken);

                return article.Id;
            }
        }
    }

    public class CreateArticleEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder builder)
        {
            builder.MapPost("api/articles", async (CreateArticleRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateArticle.Command>();

                var articleId = await sender.Send(command);

                return Results.Ok(articleId);
            });
        }
    }
}