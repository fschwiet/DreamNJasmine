using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Tests.TestDomain;
using NUnit.Framework;

namespace NJasmineTests
{
    public class SampleTest : GivenWhenThenFixture
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
        expect(() => player.IsPlaying);
        expect(() => player.CurrentSong.Equals(song));
        expect(() => !player.CurrentSong.Equals(new Song()));
    });

    describe("when song has been paused", delegate()
    {
        beforeEach(player.Pause);

        it("should indicate the song is not currently paused", delegate
        {
            expect(() => !player.IsPlaying);
            expect(() => player.CurrentSong == null);
        });

        describe("Resume", delegate
        {
            it("should indicate the song is playing", delegate()
            {
                player.Resume();
                expect(() => player.CurrentSong.Equals(song));
                expect(() => !player.CurrentSong.Equals(new Song()));
            });
        });
    });
});



        }
    }
}
