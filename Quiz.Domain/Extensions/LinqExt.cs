namespace Quiz.Domain.Extensions {

    public static class LinqExt {

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Func<TSource, bool> predicate) {
            return (condition) ? source.Where(predicate).AsQueryable() : source;
        }

    }
}
