using System.Collections.Generic;
using Engine.Storage;

namespace Engine
{
    public abstract class ColumnBase<T>: Column
    {
        protected SortedDictionary<long, T> idxValues;

        public override int Count => idxValues.Count;

        public override IReadOnlyCollection<long> Indices => idxValues.Keys;
        public IReadOnlyDictionary<long, T> IdxValues => idxValues;

        protected ColumnBase(string tablePath, string name, SortedDictionary<long, T> idxValues)
            : base(tablePath, name)
        {
            this.idxValues = idxValues ?? new SortedDictionary<long, T>();
        }

        protected void FullUpdateBase(T value)
        {
            foreach (var idx in idxValues.Keys)
                idxValues[idx] = value;
        }

        protected SortedSet<long> GetIndicesToLoad(List<long> indices)
        {
            SortedSet<long> indicesToLoad = null;

            if (indices != null)
            {
                indicesToLoad = new SortedSet<long>();
                foreach (var idx in indices)
                {
                    if (idxValues.ContainsKey(idx))
                        continue;

                    indicesToLoad.Add(idx);
                }
            }

            return indicesToLoad;
        }
        
        // Precondition: rowsToDelete is a sorted list of indices
        public override void Delete(List<long> rowsToDelete, IStorage storage)
        {
            if (rowsToDelete == null)
            {
                idxValues.Clear();
                storage.DeleteFile(GetDataFileName(TablePath, Name));
                return;
            }

            for (int i = 0; i < rowsToDelete.Count; ++i)
                idxValues.Remove(rowsToDelete[i]);
            
            DeleteInternal(rowsToDelete, storage);
        }
    }
}
