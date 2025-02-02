using WHFSocial.NoSqlModels;

namespace WFHSocial.Api.Application.Interfaces.Repository
{
    public interface IUserRepositoryNoSql
    {
        Task<PersonModel?> GetPerson();
    }
}