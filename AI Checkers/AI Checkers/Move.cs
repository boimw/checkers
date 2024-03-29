﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AICheckers
{
    class Move
    { 
        public Move()
        {
        }

        public Move(Point source, Point destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public Move(Point source, int destinationX, int destinationY)
            : this(source, new Point(destinationX, destinationY))
        {
        }

        public Move(int sourceX, int sourceY, int destinationX, int destinationY)
            : this(new Point(sourceX, sourceY), new Point(destinationX, destinationY))
        {
        }

        private Point source = new Point(-1, -1);
        private Point destination = new Point(-1, -1);
        private List<Point> captures = new List<Point>();

        public Point Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        public Point Destination
        {
            get { return this.destination; }
            set { this.destination = value; }
        }

        public List<Point> Captures
        {
            get { return captures; }
        }

        public override string ToString()
        {
            return String.Format("Source: {0}, Dest: {1}", source, destination);
        }
    }
}
