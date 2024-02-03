
- Full-Text Search Columns


select join

select in select

ook  wat dingen met JSON
array
select op prop
edit prop

CTE Support


On the other hand, if the database default value is true, this means when the property value is false, then the database default will be used, which is true! And when the property value is true, then true will be inserted. So, the value in the column will always end true in the database, regardless of what the property value is.
EF8 fixes this problem by setting the sentinel for bool properties to the same value as the database default value. Both cases above then result in the correct value being inserted, regardless of whether the database default is true or false.

boolean default true




## getRepositoryFunctions

getFirstOrNullWhere
getFirstWhere
getFirstOrNullById
getFirstById
getSingleOrNullWhere
getSingleWhere
getSingleById
updateWhere
deleteWhere
getMany
getReadAuthorizedFirstOrNullById
getReadAuthorizedMany
getWriteAuthorizedFirstOrNullById
getWriteAuthorizedMany



export function applySorting<R extends SelectQueryBuilder<any, any, any>, T extends string = string>(query: R, sortColumns: SortColumn<T>[] = []) {
    // eslint-disable-next-line @typescript-eslint/unbound-method
    const { ref } = kyselyDatabase.dynamic;
    let result = query;
    for (const sortColumn of sortColumns) {
        const direction = sortColumn.direction === SortDirections.Ascending ? 'asc' : 'desc';
        const sanitizedColumnName = ref(sanitizeIdentifier(sortColumn.columnName));

        result = result.orderBy(sanitizedColumnName, sql.raw(`${direction} nulls last`)) as R;
    }
    return result;
}


 .on('query-response', (_response, query) => {
        try {
            // eslint-disable-next-line no-underscore-dangle
            const uid = query.__knexQueryUid;
            if (times[uid]) {
                const elapsedTimeMs = performance.now()! - times[uid]!.startTime;
                if (elapsedTimeMs > 1000) {
                    if (!query.sql.includes('PostgreSQL database dump')) {
                        console.log(`Slow query. Time: ${elapsedTimeMs.toFixed(3)} ms. Query: "${query.sql}"`);
                        const context = getRequestContextObject();
                        withScope((scope) => {
                            scope.setUser({ id: context?.user?.id as string, email: context?.user?.emailAddress as string });
                            scope.setExtra('query', query.sql);
                            scope.setExtra('elapsedTimeMs', elapsedTimeMs);
                            scope.setExtra('requestPath', context?.requestPath);
                            scope.setExtra('requestQuery', context?.requestQuery);
                            scope.setExtra('contextId', context?.id);

                            captureException(new Error(`Slow query. Query: "${query.sql}"`));
                        });
                    }
                }
                delete times[uid];
            }
        } catch (error) {
            console.log('error database .on query-response:', error);
        }

  pg.types.setTypeParser(20, 'text', parseInt);
    pg.types.setTypeParser(1700, 'text', Number);
    pg.types.setTypeParser(1082, (val: Date) => (val === null ? null : moment(val).format('YYYY-MM-DD')));
    pg.types.setTypeParser(1114, (val: Date) => (val === null ? null : moment(val).toISOString()));
    pg.types.setTypeParser(1184, (val: Date) => (val === null ? null : moment(val).toISOString()));

    registerBeforeInsertTransform(
        (item: any, typedQueryBuilder: ITypedQueryBuilder<Record<string, unknown>, Record<string, unknown>, Record<string, unknown>>) => {
            if (typedQueryBuilder.columns.find((i) => i.name === 'created_at') && !Object.prototype.hasOwnProperty.call(item, 'created_at')) {
                item.created_at = new Date();
            }
            if (typedQueryBuilder.columns.find((i) => i.name === 'updated_at') && !Object.prototype.hasOwnProperty.call(item, 'updated_at')) {
                item.updated_at = new Date();
            }
            if (typedQueryBuilder.columns.find((i) => i.name === 'id') && !Object.prototype.hasOwnProperty.call(item, 'id')) {
                item.id = uuid();
            }
            return item;
        },
    );

    registerBeforeUpdateTransform(
        (item: any, typedQueryBuilder: ITypedQueryBuilder<Record<string, unknown>, Record<string, unknown>, Record<string, unknown>>) => {
            if (typedQueryBuilder.columns.find((i) => i.name === 'updated_at') && !Object.prototype.hasOwnProperty.call(item, 'updated_at')) {
                item.updated_at = new Date();
            }
            return item;
        },
    );

    