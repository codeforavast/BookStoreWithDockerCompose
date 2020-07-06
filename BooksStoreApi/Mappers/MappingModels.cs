using AutoMapper;
using BooksStore.Api.RequestModels;
using BooksStore.Api.ResponseModels;
using BookStore.Service.Models;
using System;

namespace BooksStore.Api.Mappers
{
    public class MappingModels : Profile
    {
        public MappingModels()
        {
            CreateMap<Book, BooksViewModel>();
            CreateMap<BooksRequestModel, Book>().ForMember(dest=>dest.ISBN, opt => opt.MapFrom(src => Guid.NewGuid())) ;
            CreateMap<BooksUpdateRequestModel, Book>();

        }
    }
}
