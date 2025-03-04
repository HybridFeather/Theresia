using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresia.Services.Interfaces
{
    public interface IDirectoryService
    {
        void Initialize();

        string GetRootDirectory();

        string GetMovieDirectory();

        string GetMovieCoverDirectory();

        string GetCastCrewDirectory();

        string GetVideoDirectory();

        string GetVideoCoverDirectory();
    }
}
