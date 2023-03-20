using System.Diagnostics.Metrics;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.IO;


//_______________________Test data_______________________
ListNode a = new()
{
    Next = null,
    Previous = null,
    Random = null,
    Data = "It's first item"
};

ListNode b = new()
{
    Next = null,
    Previous = a,
    Random = null,
    Data = "It's second item"
};
a.Next = b;

ListNode c = new()
{
    Next = null,
    Previous = b,
    Random = null,
    Data = "It's third item"
};
b.Next = c;
a.Random = c;
b.Random = a;
c.Random= b;

ListRandom abc = new ListRandom()
{
    Head= a,
    Tail= c,
    Count= 3,
};

Stream stream = new FileStream("test.txt",FileMode.Create,FileAccess.ReadWrite);
abc.Serialize(stream);
stream.Close();

Stream stream1 = new FileStream("test.txt", FileMode.Open, FileAccess.Read);
abc.Deserialize(stream1);
stream.Close();

//_______________________Realisation_______________________

class ListNode
{
    public ListNode Previous;
    public ListNode Next;
    public ListNode Random; // произвольный элемент внутри списка
    public string Data;
}



class ListRandom
{
    public ListNode Head;
    public ListNode Tail;
    public int Count;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="s">Stream for saving list</param>
    public void Serialize(Stream s)
    {
        //Creating dictionary for storing node and it's id
        //Node is the key, for getting his id as ref (for random node)
        Dictionary<ListNode, int> forSerializationDictionary= new();
        var counter = 0;
        
        //Adding nodes to dictionary
        for (ListNode currentNode = Head; currentNode != null; currentNode = currentNode.Next)
            forSerializationDictionary.Add(currentNode, counter++);
     
    
        //Write nodes to stream
        using (BinaryWriter writer = new BinaryWriter(s))
        {
            foreach(var pair in forSerializationDictionary)
            {
                writer.Write(pair.Key.Data); // Node data
                writer.Write(forSerializationDictionary[pair.Key.Random]);  // Id of randome node from list
            }
        }
        
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s">Stream for deserialization</param>
    public void Deserialize(Stream s)
    {
        //May be better to use Dictionary<int, KeyValuePair<int, string>> 
        //or sth like this, but for visual comfort I used 3 different Lists
        List<string> dataList = new();
        List<int> randomNodeIds = new();
        List<ListNode> nodes = new();
        
        //Reading stream and getting Data(string) and id of random node
        //(ref stores in correspond node)
        using (BinaryReader reader = new BinaryReader(s))
        {
            while (reader.PeekChar() != -1)
            {
                dataList.Add(reader.ReadString());
                randomNodeIds.Add(reader.ReadInt32());
            }
        }
        Count = dataList.Count ;
     
        for (int i = 0; i < Count; i++)
        {
            //Creating new instance every iteration
            //Might be better way to use 1 instance, but for this 
            //deserialization must work by different algorithm
            var current = new ListNode();
            current.Data = dataList[i];
            if (i != Count - 1)
                current.Next = new ListNode()
                {
                    Previous = current,
                };
            else
            {
                current.Next = null;
                Tail = current;
                
            }

            nodes.Add(current);
        }
        
        for (var i = 0; i< Count; i++)
        {
            nodes[i].Random = nodes[randomNodeIds[i]];
        }
    } // Breakpoint place for checking final values
}


