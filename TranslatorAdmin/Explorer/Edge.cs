﻿namespace Translator.Explorer
{
    internal class Edge
    {
        public readonly Node This;
        public readonly Node Child;
        private float _weight = 1.0f;
        public float Weight { get => _weight; set => _ = value > 1.0f ? _weight = value : _weight = 1.0f; }

        public Edge(Node This, Node Child)
        {
            this.This = This;
            this.Child = Child;
        }

        //new hash function so that we can create a new edge but still delete an old one with it and so on
        public override int GetHashCode()
        {
            return This.ID.GetHashCode() ^ Child.ID.GetHashCode();
        }

        public override string ToString()
        {
            return This.ID + "->" + Child.ID;
        }
    }
}