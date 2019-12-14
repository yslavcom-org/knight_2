using System.Collections.Generic;


public class ActiveFieldDomeCollectionController 
{
    private Dictionary<int, ForceFieldDomeController> forceFieldDomeActiveDictionary;

    public ActiveFieldDomeCollectionController()
    {
        forceFieldDomeActiveDictionary = new Dictionary<int, ForceFieldDomeController>();
    }

    public bool Add(int id, ForceFieldDomeController forcedField )
    {
        if (null == forceFieldDomeActiveDictionary) return false;

        if (!forceFieldDomeActiveDictionary.ContainsKey(id))
        {
            forceFieldDomeActiveDictionary.Add(id, forcedField);
        }
        return true;
    }

    public bool GetValue(int id, out ForceFieldDomeController obj)
    {
        if (null == forceFieldDomeActiveDictionary)
        {
            obj = null;
            return false;
        }
        forceFieldDomeActiveDictionary.TryGetValue(id, out obj);
        return true;
    }

    public bool Remove(int id)
    {
        if (null == forceFieldDomeActiveDictionary) return false;

        if(forceFieldDomeActiveDictionary.ContainsKey(id))
        {
            forceFieldDomeActiveDictionary.Remove(id);
        }
        return true;
    }

    public bool ClearAll()
    {
        if (null == forceFieldDomeActiveDictionary) return false;

        forceFieldDomeActiveDictionary.Clear();
        return true;
    }

    public int GetCount()
    {
        if (null == forceFieldDomeActiveDictionary) return 0;
        return forceFieldDomeActiveDictionary.Count;
    }
}
