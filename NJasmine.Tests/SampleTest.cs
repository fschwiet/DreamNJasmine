using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Tests.TestDomain;
using NUnit.Framework;

namespace NJasmineTests
{
    public class SampleTest : NJasmineFixture
    {
        public override void Tests()
        {



describe("player", delegate()
{
    var player = new Player();

    forEach(delegate()
    {
        return new Song[] {new Song(), new OggSong(), new Mp3Song()};
    }, 
    delegate(Song song) 
    {

        it("should be able to play a song", delegate()
        {
            player.Play(song);

            expect(player.CurrentSong).to.Equal(song);
            expect(player.CurrentSong).not.to.Equal(new Song());
        });

        describe("when song has been paused", delegate()
        {
            beforeEach(delegate()
            {
                player.Play(song);
                player.Pause();
            });

            it("should indicate the song is currently paused", delegate()
            {
                expect(player.IsPlaying).to.Equal(false);
            });

            it("should not indicate a current song", delegate()
            {
                expect(player.CurrentSong).to.Be.Null();
            });

            describe("Resume", delegate()
            {
                beforeEach(player.Resume);

                it("should indicate the song is playing", delegate()
                {
                    expect(player.IsPlaying).to.Equal(true);
                    expect(player.CurrentSong).to.Equal(song);
                });
            });
        });
    });
});



        }
    }
}
