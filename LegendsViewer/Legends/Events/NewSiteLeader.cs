using System;
using System.Collections.Generic;
using System.Linq;
using LegendsViewer.Legends.Parser;
using LegendsViewer.Legends.WorldObjects;

namespace LegendsViewer.Legends.Events
{
    public class NewSiteLeader : WorldEvent
    {
        public Entity Attacker, Defender, SiteEntity, NewSiteEntity;
        public Site Site;
        public HistoricalFigure NewLeader;
        public NewSiteLeader(List<Property> properties, World world)
            : base(properties, world)
        {
            foreach (Property property in properties)
            {
                switch (property.Name)
                {
                    case "attacker_civ_id": Attacker = world.GetEntity(Convert.ToInt32(property.Value)); break;
                    case "defender_civ_id": Defender = world.GetEntity(Convert.ToInt32(property.Value)); break;
                    case "site_civ_id": SiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                    case "site_id": Site = world.GetSite(Convert.ToInt32(property.Value)); break;
                    case "new_site_civ_id": NewSiteEntity = world.GetEntity(Convert.ToInt32(property.Value)); break;
                    case "new_leader_hfid": NewLeader = world.GetHistoricalFigure(Convert.ToInt32(property.Value)); break;
                }
            }

            if (Site.OwnerHistory.Count == 0)
            {
                if (SiteEntity != null && SiteEntity != Defender)
                {
                    SiteEntity.Parent = Defender;
                    Site.OwnerHistory.Add(new OwnerPeriod(Site, SiteEntity, -1, "founded"));
                }
                else
                {
                    Site.OwnerHistory.Add(new OwnerPeriod(Site, Defender, -1, "founded"));
                }
            }

            Site.OwnerHistory.Last().EndCause = "taken over";
            Site.OwnerHistory.Last().EndYear = Year;
            Site.OwnerHistory.Last().Ender = Attacker;
            NewSiteEntity.Parent = Attacker;
            Site.OwnerHistory.Add(new OwnerPeriod(Site, NewSiteEntity, Year, "took over"));

            Attacker.AddEvent(this);
            Defender.AddEvent(this);
            if (SiteEntity != Defender)
            {
                SiteEntity.AddEvent(this);
            }

            Site.AddEvent(this);
            NewSiteEntity.AddEvent(this);
            NewLeader.AddEvent(this);
        }
        public override string Print(bool link = true, DwarfObject pov = null)
        {
            string eventString = GetYearTime() + Attacker.ToLink(link, pov, this) + " defeated ";
            if (SiteEntity != null && SiteEntity != Defender)
            {
                eventString += SiteEntity.ToLink(link, pov, this) + " of ";
            }

            eventString += Defender.ToLink(link, pov, this) + " and placed " + NewLeader.ToLink(link, pov, this) + " in charge of " + Site.ToLink(link, pov, this) + ". The new government was called " + NewSiteEntity.ToLink(link, pov, this);
            eventString += PrintParentCollection(link, pov);
            eventString += ".";
            return eventString;
        }
    }
}