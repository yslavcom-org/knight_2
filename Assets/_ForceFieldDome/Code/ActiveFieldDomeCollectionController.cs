using System;
using System.Collections;
using System.Collections.Generic;


public class ActiveFieldDomeCollectionController 
{
    private Dictionary<int, ForceFieldDomeController> collection;

    public ActiveFieldDomeCollectionController()
    {
        collection = new Dictionary<int, ForceFieldDomeController>();
    }

    public bool Add(int id, ForceFieldDomeController forcedField )
    {
        if (null == collection) return false;

        if (!collection.ContainsKey(id))
        {
            collection.Add(id, forcedField);
        }
        return true;
    }

    public bool GetValue(int id, out ForceFieldDomeController obj)
    {
        if (null == collection)
        {
            obj = null;
            return false;
        }
      
        return collection.TryGetValue(id, out obj); 
    }

    public bool GetFirstPair(out int id, out ForceFieldDomeController obj)
    {
        if(null == collection)
        {
            id = 0;
            obj = null;
            return false;
        }

        IDictionaryEnumerator myEnumerator = collection.GetEnumerator();
        if(myEnumerator.MoveNext())
        {
            id = (int)myEnumerator.Key;
            obj = (ForceFieldDomeController)myEnumerator.Value;
            return true;
        }
        else
        {
            id = 0;
            obj = null;
            return false;
        }
    }

    public bool Remove(int id)
    {
        if (null == collection) return false;

        if(collection.ContainsKey(id))
        {
            collection.Remove(id);
        }
        return true;
    }

    public bool ClearAll()
    {
        if (null == collection) return false;

        collection.Clear();
        return true;
    }

    public int GetCount()
    {
        if (null == collection) return 0;
        return collection.Count;
    }
}
