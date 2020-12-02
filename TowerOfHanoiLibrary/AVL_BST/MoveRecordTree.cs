using System;

namespace TowerOfHanoiLibrary.AVL_BST
{
    public class MoveRecordTree<Tkey, Tvalue> where Tkey : IComparable<Tkey>
    {
        internal Node<Tkey, Tvalue> Root;
        public delegate void RecordDisplay(Tvalue val);

        public MoveRecordTree()
        {
            Root = null;
        }

        public void Insert(Tkey key, Tvalue value)
        {
            Root = Insert(Root, key, value);
        }

        private Node<Tkey, Tvalue> Insert(Node<Tkey, Tvalue> parent, Tkey key, Tvalue value)
        {
            if (parent == null)
            {
                parent = new Node<Tkey, Tvalue>(key, value);
            }
            else if (key.CompareTo(parent.Key) < 0) // key is less than parent.key
            {
                parent.Left = Insert(parent.Left, key, value);
                parent = BalanceTree(parent);
            }
            else
            {
                parent.Right = Insert(parent.Right, key, value);
                parent = BalanceTree(parent);
            }
            return parent;
        }

        //Traverse visits each node in ascending order.  
        public void Traverse(RecordDisplay recordDisplay)
        {
            Traverse(Root, recordDisplay);
        }
        private void Traverse(Node<Tkey, Tvalue> node, RecordDisplay recordDisplay)
        {
            if (node == null) return;
            Traverse(node.Left, recordDisplay);
            recordDisplay(node.Value);
            Traverse(node.Right, recordDisplay);
        }

        // Traverse in Descending order
        public void TraverseReverse(RecordDisplay recordDisplay)
        {
            TraverseReverse(Root, recordDisplay);
        }

        private void TraverseReverse(Node<Tkey, Tvalue> node, RecordDisplay recordDisplay)
        {
            if (node == null) return;
            TraverseReverse(node.Right, recordDisplay);
            recordDisplay(node.Value);
            TraverseReverse(node.Left, recordDisplay);
        }

        public Tvalue Find(Tkey key)
        {
            return Find(Root, key);
        }

        private Tvalue Find(Node<Tkey, Tvalue> node, Tkey key)
        {
            if (node == null)
            {
                return default(Tvalue);
            }

            if (key.CompareTo(node.Key) == 0)
            {
                return node.Value;
            }

            if (key.CompareTo(node.Key) > 0)
            {
                if (node.Right == null)
                {
                    return default(Tvalue);
                }
                else
                {
                    node = node.Right;
                    return Find(node, key);
                }
            }
            else
            {
                if (node.Left == null)
                {
                    return default(Tvalue);
                }
                else
                {
                    node = node.Left;
                    return Find(node, key);
                }
            }
        }

        public void Delete(Tkey key)
        {
            Root = Delete(Root, key);
        }


        //This logic is similar to the insert logic in that after a successful delete
        //occurs the table is rebalanced using the same methods.

        private Node<Tkey, Tvalue> Delete(Node<Tkey, Tvalue> node, Tkey key)
        {
            if (node == null) return node;

            if (key.CompareTo(node.Key) < 0) // key is less than node.key
            {
                node.Left = Delete(node.Left, key);
                node = BalanceTree(node);

            }
            else if (key.CompareTo(node.Key) > 0)   // key is greater than node.key
            {
                node.Right = Delete(node.Right, key);
                node = BalanceTree(node);
            }
            else
            {
                //Case where node has zero or one child.  Just delete it.
                if (node.Right == null)
                {
                    node = node.Left;
                    if (node != null)
                    {
                        node = BalanceTree(node);
                    }
                }
                else if (node.Left == null)
                {
                    node = node.Right;
                    if (node != null)
                    {
                        node = BalanceTree(node);
                    }
                }
                else
                {
                    node.Key = MaxLeftChildValue(node.Left);
                    node.Left = Delete(node.Left, node.Key);
                    node = BalanceTree(node);
                }
            }

            return node;
        }

        private Tkey MaxLeftChildValue(Node<Tkey, Tvalue> node)
        {
            Tkey maxVal = node.Key;
            while (node.Right != null)
            {
                maxVal = node.Right.Key;
                node = node.Right;
            }

            return maxVal;
        }

        /**
         * Find the height of the binary tree given the root node.
         * @return Height of the longest path.
         */
        private int ComputeHeight(Node<Tkey, Tvalue> root)
        {
            if (null == root)
            {
                return 0;
            }
            int leftHeight = ComputeHeight(root.Left);
            int rightHeight = ComputeHeight(root.Right);
            return leftHeight > rightHeight ? leftHeight + 1 : rightHeight + 1;
        }


        #region "Balancing Methods"

        private Node<Tkey, Tvalue> BalanceTree(Node<Tkey, Tvalue> node)
        {
            int balanceFactor = GetBalanceFactor(node);
            if (balanceFactor > 1)  //Left subtree is taller than right subtree
            {
                if (GetBalanceFactor(node.Left) > 0)  //Left.left is taller than left.right
                {
                    node = RotateRight(node);
                }
                else
                {
                    node = RotateLeftRight(node);
                }
            }
            else if (balanceFactor < -1) //Right subtree is taller than left
            {
                if (GetBalanceFactor(node.Right) > 0)  //Right.left is taller than right.right.
                {
                    node = RotateRightLeft(node);
                }
                else
                {
                    node = RotateLeft(node);
                }
            }
            return node;
        }

        private Node<Tkey, Tvalue> RotateRight(Node<Tkey, Tvalue> curr)
        {
            Node<Tkey, Tvalue> pivot = curr.Left;
            curr.Left = pivot.Right;
            pivot.Right = curr;
            return pivot;
        }

        private Node<Tkey, Tvalue> RotateLeft(Node<Tkey, Tvalue> curr)
        {
            Node<Tkey, Tvalue> pivot = curr.Right;
            curr.Right = pivot.Left;
            pivot.Left = curr;
            return pivot;
        }

        private Node<Tkey, Tvalue> RotateRightLeft(Node<Tkey, Tvalue> curr)
        {
            Node<Tkey, Tvalue> pivot = curr.Right;
            curr.Right = RotateRight(pivot);
            curr = RotateLeft(curr);
            return curr;
        }

        private Node<Tkey, Tvalue> RotateLeftRight(Node<Tkey, Tvalue> curr)
        {
            Node<Tkey, Tvalue> pivot = curr.Left;
            curr.Left = RotateLeft(pivot);
            return RotateRight(curr);
        }

        private int GetBalanceFactor(Node<Tkey, Tvalue> node)
        {
            int l = GetHeight(node.Left);
            int r = GetHeight(node.Right);
            int balanceFactor = l - r;
            return balanceFactor;
        }

        private int GetHeight(Node<Tkey, Tvalue> node)
        {
            int height = 0;
            if (node != null)
            {
                int l = GetHeight(node.Left);
                int r = GetHeight(node.Right);
                int m = l > r ? l : r;
                height = m + 1;
            }
            return height;
        }

        #endregion

    }
}
