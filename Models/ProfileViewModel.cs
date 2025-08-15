using GymTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Models
{
    internal class ProfileViewModel
    {
        public Services.Profile Profile { get; set; }
        public ProfileViewModel()
        {
            Profile = AppState.Profile;
        }
    }
}
