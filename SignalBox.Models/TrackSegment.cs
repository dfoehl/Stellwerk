using System;
using System.Collections.Generic;
using System.Text;

namespace SignalBox.Models
{
    public abstract class TrackSegment : SignalBoxElement
    {
        public bool IsFree { get => State == TrackState.Free; }
        public TrackState State {get; set;}

        protected TrackSegment(SignalBox signalBox) : base(signalBox)
        {
        }
    }

    public enum TrackState
    {
        Free,
        Allocating,
        Allocated,
        Blocked
    }
}
