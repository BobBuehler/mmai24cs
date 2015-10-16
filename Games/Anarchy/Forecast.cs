// Generated by Creer at 10:54PM on October 16, 2015 UTC, git hash: '98604e3773d1933864742cb78acbf6ea0b4ecd7b'
// The weather effect that will be applied at the end of a turn, which causes fires to spread.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// <<-- Creer-Merge: usings -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
// you can add addtional using(s) here
// <<-- /Creer-Merge: usings -->>

namespace Joueur.cs.Games.Anarchy
{
    /// <summary>
    /// The weather effect that will be applied at the end of a turn, which causes fires to spread.
    /// </summary>
    class Forecast : Anarchy.GameObject
    {
        #region Properties
        /// <summary>
        /// The Player that can use WeatherStations to control this Forecast when its the nextForecast.
        /// </summary>
        public Anarchy.Player ControllingPlayer { get; protected set; }

        /// <summary>
        /// The direction the wind will blow fires in. Can be 'north', 'east', 'south', or 'west'
        /// </summary>
        public string Direction { get; protected set; }

        /// <summary>
        /// How much of a Building's fire that can be blown in the direction of this Forecast. Fire is duplicated (copied), not moved (transfered).
        /// </summary>
        public int Intensity { get; protected set; }


        // <<-- Creer-Merge: properties -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add addtional properties(s) here. None of them will be tracked or updated by the server.
        // <<-- /Creer-Merge: properties -->>
        #endregion


        #region Methods
        /// <summary>
        /// Creates a new instance of {$obj_key}. Used during game initialization, do not call directly.
        /// </summary>
        protected Forecast() : base()
        {
        }


        // <<-- Creer-Merge: methods -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add addtional method(s) here.
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}
