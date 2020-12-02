namespace TowerOfHanoiLibrary.AVL_BST
{
    internal class Node<Tkey, Tvalue>
    {
        public Tkey Key { get; set; }
        public Tvalue Value { get; set; }
        public Node<Tkey, Tvalue> Left { get; set; }
        public Node<Tkey, Tvalue> Right { get; set; }

        public Node()
        {
            Left = null;
            Right = null;
        }

        public Node(Tkey key, Tvalue value) : this()
        {
            Key = key;
            Value = value;
        }
    }
}
