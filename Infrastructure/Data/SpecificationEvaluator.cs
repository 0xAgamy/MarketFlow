using Core.Entities;
using Core.Interfaces;
namespace Infrastructure.Data
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> spec)
        {

            var result= spec.ApplyCriteria(query);

            result= ApplyOrdering(result,spec);
            if (spec.IsDistinct)
                result=result.Distinct();

            if (spec.IsPagingEnabled) result = result.Skip(spec.Skip).Take(spec.Take);

             return result;
                  

        }

        public static IQueryable<TResult> GetQuery<TSepc,TResult>(IQueryable<T> query, ISpecification<T,TResult> spec)
        {
             var queryable = GetQuery(query, spec);

            var result = spec.Select is null
                ? queryable.Cast<TResult>()
                : queryable.Select(spec.Select);

            return result;
        }


        private static IQueryable<T> ApplyOrdering(IQueryable<T> query, ISpecification<T> spec)
        {
            if (spec.OrderByDescending is not null)
                return query.OrderByDescending(spec.OrderByDescending);
            if (spec.OrderBy is not null)
                return query.OrderBy(spec.OrderBy);
            return query;
        }
    }
}
