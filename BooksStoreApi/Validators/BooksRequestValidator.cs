using BooksStore.Api.RequestModels;
using FluentValidation;
using System;

namespace BooksStore.Api.Validators
{
    public class BooksRequestValidator : AbstractValidator<BooksRequestModel>
    {
        public BooksRequestValidator()
        {
            RuleFor(x => x.Author).NotNull().Length(1,255);
            RuleFor(x => x.Title).NotNull().Length(1,255);
            RuleFor(x => x.Genre).NotNull().Length(1,50);
            RuleFor(x => x.PublishedOn).InclusiveBetween(new DateTime(1800,1,1), DateTime.Now);
        }
    }
}
