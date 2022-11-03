using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class VStackItemSource : MonoBehaviour
{
    public GameObject ItemPrefab;
    public GameObject ContentPanel;
    List<GameObject> children = new List<GameObject>();
    public void SetItems(Dictionary<long, ServerResponse> items)
	{
        ClearItems();
        foreach(var i in items)
		{
            var cell = Instantiate<GameObject>(ItemPrefab);
            var serverItem = cell.GetComponent<ServerListItem>();
            serverItem.Id = i.Key;
            serverItem.ServerResponse = i.Value;

            var cellz = cell.transform.position.z;
            var parentZ = ContentPanel.transform.position.z;
            cell.transform.SetParent(ContentPanel.transform, false);
            children.Add(cell);
		}

    }
    void ClearItems()
	{
        var children = this.children.ToList();
        this.children.Clear();
        foreach(var c in children)
		{
            Destroy(c);
		}
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
