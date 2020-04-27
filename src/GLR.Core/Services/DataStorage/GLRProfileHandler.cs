using System.Collections.Generic;
using GLR.Core.Entities;

namespace GLR.Core.Services.DataStorage
{
    public class GLRProfileHandler
    {
        private LiteDBHandler _db;

        public GLRProfileHandler(LiteDBHandler db)
        {
            _db = db;
        }

        public Profile RetrieveProfile(ulong id)
            => _db.RestoreSingle<Profile>(x => x.Id == id);

        public List<Profile> RetrieveManyProfiles(IEnumerable<ulong> ids)
        {
            var profiles = new List<Profile>();

            foreach (var id in ids)
            {
                var profile = RetrieveProfile(id);
                profiles.Add(profile);
            }

            return profiles;
        }

        public void StoreProfile(Profile profile)
        {
            if (_db.Exists<Profile>(x => x.Id == profile.Id))
                _db.Update(profile);
            else _db.Store<Profile>(profile);
        }
    }
}
