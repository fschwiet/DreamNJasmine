using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Tests.TestDomain;
using NUnit.Framework;
using Should.Fluent;

namespace NJasmineTests
{
    public class SampleTest : NJasmineFixture
    {
        public override void Specify()
        {
            


describe("player", delegate()
{
    var player = new Player();
    var song = new Song();

    beforeEach(delegate {
        player.Play(song);
    });

    it("should be able to play the song", delegate()
    {
        player.IsPlaying.Should().Equal(true);
        player.CurrentSong.Should().Equal(song);
        player.CurrentSong.Should().Not.Equal(new Song());
    });

    describe("when song has been paused", delegate()
    {
        beforeEach(player.Pause);

        it("should indicate the song is not currently paused", delegate
        {
            player.IsPlaying.Should().Equal(false);
            player.CurrentSong.Should().Be.Null();
        });

        describe("Resume", delegate
        {
            it("should indicate the song is playing", delegate()
            {
                player.Resume();
                player.CurrentSong.Should().Equal(song);
                player.CurrentSong.Should().Not.Equal(new Song());
            });
        });
    });
});



        }
    }
}
