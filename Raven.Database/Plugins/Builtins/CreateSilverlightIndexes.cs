using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;

namespace Raven.Database.Plugins.Builtins
{
	public class CreateSilverlightIndexes : ISilverlightRequestedAware
	{
		public void SilverlightWasRequested(DocumentDatabase database)
		{
			if (database.Indexes.GetIndexDefinition(Constants.DocumentsByEntityNameIndex) == null)
			{
                database.Indexes.PutIndex(Constants.DocumentsByEntityNameIndex, new IndexDefinition
				{
					Map = @"from doc in docs 
let Tag = doc[""@metadata""][""Raven-Entity-Name""]
select new { Tag, LastModified = (DateTime)doc[""@metadata""][""Last-Modified""] , LastModifiedTicsk = (DateTime)doc[""@metadata""][""Last-Modified""].Ticks };",
					Indexes =
					{
						{"Tag", FieldIndexing.NotAnalyzed},
						{"LastModified", FieldIndexing.NotAnalyzed},
                        {"LastModifiedTicks", FieldIndexing.NotAnalyzed}
					},
					SortOptions =
                    {
                        {"LastModified",SortOptions.String},
                        {"LastModifiedTicks", SortOptions.Long}
                    },

					DisableInMemoryIndexing = true
				});
			}
		}
	}
}