using System;
using System.Collections.Generic;
using System.Linq;

namespace MD.Net
{
    public class ActionBuilder : IActionBuilder
    {
        public ActionBuilder(IFormatManager formatManager)
        {
            this.FormatManager = formatManager;
        }

        public IFormatManager FormatManager { get; private set; }

        public IActions GetActions(IDevice device, IDisc currentDisc, IDisc updatedDisc)
        {
            var actions = new List<IAction>();
            if (!string.Equals(currentDisc.Title, updatedDisc.Title, StringComparison.OrdinalIgnoreCase))
            {
                actions.Add(new UpdateDiscTitleAction(device, currentDisc, updatedDisc));
            }
            this.UpdateTracks(actions, device, currentDisc, updatedDisc);
            this.RemoveTracks(actions, device, currentDisc, updatedDisc);
            this.AddTracks(actions, device, currentDisc, updatedDisc);
            return new Actions(device, currentDisc, updatedDisc, actions);
        }

        protected virtual void AddTracks(IList<IAction> actions, IDevice device, IDisc currentDisc, IDisc updatedDisc)
        {
            foreach (var updatedTrack in updatedDisc.Tracks.OrderBy(track => track.Position))
            {
                var found = default(bool);
                foreach (var currentTrack in currentDisc.Tracks)
                {
                    if (updatedTrack.Id != currentTrack.Id)
                    {
                        continue;
                    }
                    found = true;
                    break;
                }
                if (!found)
                {
                    this.AddTrack(actions, device, currentDisc, updatedDisc, updatedTrack);
                }
            }
        }

        protected virtual void AddTrack(IList<IAction> actions, IDevice device, IDisc currentDisc, IDisc updatedDisc, ITrack track)
        {
            actions.Add(new AddTrackAction(this.FormatManager, device, currentDisc, updatedDisc, track));
        }

        protected virtual void UpdateTracks(IList<IAction> actions, IDevice device, IDisc currentDisc, IDisc updatedDisc)
        {
            foreach (var currentTrack in currentDisc.Tracks.OrderBy(track => track.Position))
            {
                foreach (var updatedTrack in updatedDisc.Tracks)
                {
                    if (currentTrack.Id != updatedTrack.Id)
                    {
                        continue;
                    }
                    this.UpdateTrack(actions, device, currentDisc, updatedDisc, currentTrack, updatedTrack);
                    break;
                }
            }
        }

        protected virtual void UpdateTrack(IList<IAction> actions, IDevice device, IDisc currentDisc, IDisc updatedDisc, ITrack currentTrack, ITrack updatedTrack)
        {
            if (!string.Equals(currentTrack.Name, updatedTrack.Name, StringComparison.OrdinalIgnoreCase))
            {
                actions.Add(new UpdateTrackNameAction(device, currentDisc, updatedDisc, currentTrack, updatedTrack));
            }
        }

        protected virtual void RemoveTracks(IList<IAction> actions, IDevice device, IDisc currentDisc, IDisc updatedDisc)
        {
            foreach (var currentTrack in currentDisc.Tracks.OrderByDescending(track => track.Position))
            {
                var found = default(bool);
                foreach (var updatedTrack in updatedDisc.Tracks)
                {
                    if (currentTrack.Id != updatedTrack.Id)
                    {
                        continue;
                    }
                    found = true;
                    break;
                }
                if (!found)
                {
                    this.RemoveTrack(actions, device, currentDisc, updatedDisc, currentTrack);
                }
            }
        }

        protected virtual void RemoveTrack(IList<IAction> actions, IDevice device, IDisc currentDisc, IDisc updatedDisc, ITrack track)
        {
            actions.Add(new RemoveTrackAction(device, currentDisc, updatedDisc, track));
        }
    }
}
