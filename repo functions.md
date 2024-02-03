



- ` Task AddEmployeeAsync(Employee employee);` only add partial data, not all fields are required
- `Task<List<Employee>> GetEmployeesByCityAsync(string city);` return partial data
-  `Task<List<Project>> GetProjectsByEmployeeIdAsync(Guid employeeId);` return partial data
- 


- public async Task<IEnumerable<UserDto>> GetUsersByCityAsync(string city)
- public async Task UpdateUserDetachedAsync(int userId, string newCity, bool isActive)
-   public async Task AddUserAsync(string name, string city)
-  public async Task DeleteUserByIdAsync(int userId)


This is the list that I'm going for:

- AddEmployee 
- UpdateEmployeeName(string name)
- DeleteEmployeeById(int employeeId)
- GetEmployeesByCity(string city)
- GetProjectsByEmployeeId(int employeeId)
- GetProjectsByCustomerId(int customerId)


- Full-Text Search
- outer join
- select in select
- edit prop in JSON
- select on prop in JSON
- CTE example
- how to handle `IsActive bit NOT NULL DEFAULT 1` when creating with partial object data (not providing IsActive)
- transaction for one operation
- transaction for multiple operations
- bulk insert
- bulk update
- Dynamic Query Generation: add dictionary of filters and sort columns
- Paging and Sorting:
- self join
- aggregate function
- select view
- call stored procedure
- migrations (is there a simple file based approach? Like 0001.sql, 0002.sql, etc)
- spatial select
- appending to an array within a JSON object.


Can you generate a list of functions that you would expect to see in a repository class? Add the function signature and a brief description of what it does and what we're showcasing.


Can you think of more stuff to add to the list?




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

