// An eager young person that wants to join your gang, and will call in the veteran Cowboys you need to win the brawl in the saloon.

// DO NOT MODIFY THIS FILE
// Never try to directly create an instance of this class, or modify its member variables.
// Instead, you should only be reading its variables and calling its functions.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// <<-- Creer-Merge: usings -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
// you can add addtional using(s) here
// <<-- /Creer-Merge: usings -->>

namespace Joueur.cs.Games.Saloon
{
    /// <summary>
    /// An eager young person that wants to join your gang, and will call in the veteran Cowboys you need to win the brawl in the saloon.
    /// </summary>
    class YoungGun : Saloon.GameObject
    {
        #region Properties
        /// <summary>
        /// True if the YoungGun can call in a Cowboy, false otherwise.
        /// </summary>
        public bool CanCallIn { get; protected set; }

        /// <summary>
        /// The Player that owns and can control this YoungGun.
        /// </summary>
        public Saloon.Player Owner { get; protected set; }

        /// <summary>
        /// The Tile this YoungGun is currently on. Cowboys they send in will be on the nearest non-balcony Tile.
        /// </summary>
        public Saloon.Tile Tile { get; protected set; }


        // <<-- Creer-Merge: properties -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add addtional properties(s) here. None of them will be tracked or updated by the server.
        // <<-- /Creer-Merge: properties -->>
        #endregion


        #region Methods
        /// <summary>
        /// Creates a new instance of YoungGun. Used during game initialization, do not call directly.
        /// </summary>
        protected YoungGun() : base()
        {
        }

        /// <summary>
        /// Tells the YoungGun to call in a new Cowbow of the given job to the open Tile nearest to them.
        /// </summary>
        /// <param name="job">The job you want the Cowboy being brought to have.</param>
        /// <returns>The new Cowboy that was called in if valid. They will not be added to any `cowboys` lists until the turn ends. Null otherwise.</returns>
        public Saloon.Cowboy CallIn(string job)
        {
            return this.RunOnServer<Saloon.Cowboy>("callIn", new Dictionary<string, object> {
                {"job", job}
            });
        }


        // <<-- Creer-Merge: methods -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add addtional method(s) here.
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}
