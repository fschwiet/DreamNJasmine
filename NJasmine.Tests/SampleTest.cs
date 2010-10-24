using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Tests.TestDomain;

namespace NJasmine.Tests
{
    public class Class1 : NJasmineFixture
    {
        public override void RootDescribe(Action action)
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

            expect(player.CurrentSong).toBe.SameAs(song);
            expect(player.CurrentSong).not.toBe.SameAs(new Song());
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
                expect(player.IsPlaying).toBe.SameAs(false);
            });

            it("should not indicate a current song", delegate()
            {
                expect(player.CurrentSong).toBe.Null();
            });

            describe("Resume", delegate()
            {
                beforeEach(player.Resume);

                it("should indicate the song is playing", delegate()
                {
                    expect(player.IsPlaying).toBe.SameAs(true);
                    expect(player.CurrentSong).toBe.SameAs(song);
                });
            });
        });
    });
});



        }
    }
}
