using AutoMapper;
using BookStore.Service.Models;
using Db = BookStore.Data.Models;

namespace BookStore.Service.Mappers
{
    public class MappingModels : Profile
    {
        public MappingModels()
        {
            CreateMap<Book, Db.Book>().ReverseMap();
        }
    }
}
