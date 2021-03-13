using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dotnetcorebot.Modules.infoclasses;
using ImageProcessor;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using Sentry;
using SIlverCraftBot;
using SIlverCraftBot.Modules;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Image = System.Drawing.Image;

namespace dotnetcorebot.Modules
{
    public class Imagemodule : ModuleBase<SocketCommandContext>
    {
        private static Config dconfig = new Config();

        public static void SetConfig(Config GetConfig)
        {
            dconfig = GetConfig;
        }

        [Command("jpeg", RunMode = RunMode.Async)]
        public async Task Jpegize()
        {
            System.Collections.Generic.IReadOnlyCollection<Attachment> attachments = Context.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync("You didnt attach a image");
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync("You attached more than one image");
                return;
            }
            using WebClient myWebClient = new WebClient();
            using MemoryStream outStream = Make_jpegnised(myWebClient.DownloadData(attachments.ElementAt(0).Url));
            await Context.Channel.SendFileAsync(outStream, "silverbotimage.jpeg", "There ya go a jpegnized image");
        }

        [Command("jpeg", RunMode = RunMode.Async)]
        public async Task Jpegize(string urmom)
        {
            using WebClient myWebClient = new WebClient();
            try
            {
                using MemoryStream outStream = Make_jpegnised(myWebClient.DownloadData(urmom));
                await Context.Channel.SendFileAsync(outStream, "silverbotimage.jpeg", "There ya go a jpegnized image");
            }
            catch (ArgumentNullException nullers)
            {
                SentrySdk.CaptureException(nullers);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(dconfig.Log_channel) as SocketTextChannel;
                b.WithTitle("Error at executing jpeg");
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + nullers);
                b.AddField("Exception Message", nullers.Message);
                b.AddField("Exeption ParamaterName", nullers.ParamName);
                b.AddField("Exeption InnerExeption", nullers.InnerException);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "jpeg with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
            catch (WebException web)
            {
                SentrySdk.CaptureException(web);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(Convert.ToUInt64(dconfig.Log_channel)) as SocketTextChannel;
                b.WithTitle("Error at executing jpeg");
                Console.WriteLine(web);
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + web);
                b.AddField("Exception Message", web.Message);
                b.AddField("Exeption Response", web.Response);
                b.AddField("Exeption Status", web.Status);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "jpeg with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
        }

        private static MemoryStream Make_jpegnised(byte[] photoBytes)
        {
            ISupportedImageFormat format = new JpegFormat { Quality = 1 };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Format(format)
                            .Save(outStream);
            }
            return outStream;
        }

        private async Task Send_img_plsAsync(string gay)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithTitle("Send an image my guy");
            b.WithDescription(gay);
            b.WithFooter("Requested by " + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
            await ReplyAsync(embed: b.Build());
        }

        [Command("tint", RunMode = RunMode.Async)]
        public async Task Tint(string color)
        {
            System.Collections.Generic.IReadOnlyCollection<Attachment> attachments = Context.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync("You didnt attach a image").ConfigureAwait(false);
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync("You attached more than one image").ConfigureAwait(false);
                return;
            }
            await Context.Channel.SendFileAsync(Tint(new WebClient().DownloadData(attachments.ElementAt(0).Url), color), "silverbotimage.png", "There ya go a tinted image");
        }

        [Command("tint", RunMode = RunMode.Async)]
        public async Task Tint(string urmom, string color)
        {
            try
            {
                await Context.Channel.SendFileAsync(Tint(new WebClient().DownloadData(urmom), color), "silverbotimage.jpeg", "There ya go a tinted image");
            }
            catch (ArgumentNullException nullers)
            {
                SentrySdk.CaptureException(nullers);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(dconfig.Log_channel) as SocketTextChannel;
                b.WithTitle("Error at executing tint");
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + nullers);
                b.AddField("Exception Message", nullers.Message);
                b.AddField("Exeption ParamaterName", nullers.ParamName);
                b.AddField("Exeption InnerExeption", nullers.InnerException);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "tint with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
            catch (WebException web)
            {
                SentrySdk.CaptureException(web);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(dconfig.Log_channel) as SocketTextChannel;
                b.WithTitle("Error at executing tint");
                Console.WriteLine(web);
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + web);
                b.AddField("Exception Message", web.Message);
                b.AddField("Exeption Response", web.Response);
                b.AddField("Exeption Status", web.Status);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "tint with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
        }

        private static MemoryStream Tint(byte[] photoBytes, string color)
        {
            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Tint(ColorTranslator.FromHtml(color))
                            .Format(format)
                            .Save(outStream);
            }
            return outStream;
        }

        [Command("silver", RunMode = RunMode.Async)]
        public async Task Silver()
        {
            System.Collections.Generic.IReadOnlyCollection<Attachment> attachments = Context.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync("You didnt attach a image").ConfigureAwait(false);
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync("You attached more than one image").ConfigureAwait(false);
                return;
            }
            await Context.Channel.SendFileAsync(Filter(new WebClient().DownloadData(attachments.ElementAt(0).Url), MatrixFilters.GreyScale), "silverbotimage.png", "There ya go a silver image");
        }

        [Command("blur", RunMode = RunMode.Async)]
        public async Task Blur(int size)
        {
            System.Collections.Generic.IReadOnlyCollection<Attachment> attachments = Context.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync("You didnt attach a image").ConfigureAwait(false);
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync("You attached more than one image").ConfigureAwait(false);
                return;
            }
            await Context.Channel.SendFileAsync(Blur(new WebClient().DownloadData(attachments.ElementAt(0).Url), size), "silverbotimage.png", "There ya go a blured image");
        }

        [Command("blur", RunMode = RunMode.Async)]
        public async Task Blur(string urmom, int size)
        {
            try
            {
                await Context.Channel.SendFileAsync(Blur(new WebClient().DownloadData(urmom), size), "silverbotimage.jpeg", "There ya go a blured image");
            }
            catch (ArgumentNullException nullers)
            {
                SentrySdk.CaptureException(nullers);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(Convert.ToUInt64(dconfig.Log_channel)) as SocketTextChannel;
                b.WithTitle("Error at executing blur");
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + nullers);
                b.AddField("Exception Message", nullers.Message);
                b.AddField("Exeption ParamaterName", nullers.ParamName);
                b.AddField("Exeption InnerExeption", nullers.InnerException);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "blur with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
            catch (WebException web)
            {
                SentrySdk.CaptureException(web);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(Convert.ToUInt64(dconfig.Log_channel)) as SocketTextChannel;
                b.WithTitle("Error at executing blur");
                Console.WriteLine(web);
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + web);
                b.AddField("Exception Message", web.Message);
                b.AddField("Exeption Response", web.Response);
                b.AddField("Exeption Status", web.Status);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "blur with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
        }

        private static MemoryStream Blur(byte[] photoBytes, int size)
        {
            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .GaussianBlur(size)
                            .Format(format)
                            .Save(outStream);
            }
            return outStream;
        }

        [Command("silver", RunMode = RunMode.Async)]
        public async Task Silver(string urmom)
        {
            try
            {
                await Context.Channel.SendFileAsync(Filter(new WebClient().DownloadData(urmom), MatrixFilters.GreyScale), "silverbotimage.png", "There ya go a silver image");
            }
            catch (ArgumentNullException nullers)
            {
                SentrySdk.CaptureException(nullers);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(Convert.ToUInt64(dconfig.Log_channel)) as SocketTextChannel;
                b.WithTitle("Error at executing tint");
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + nullers);
                b.AddField("Exception Message", nullers.Message);
                b.AddField("Exeption ParamaterName", nullers.ParamName);
                b.AddField("Exeption InnerExeption", nullers.InnerException);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "silver with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
            catch (WebException web)
            {
                SentrySdk.CaptureException(web);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(Convert.ToUInt64(dconfig.Log_channel)) as SocketTextChannel;
                b.WithTitle("Error at executing tint");
                Console.WriteLine(web);
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + web);
                b.AddField("Exception Message", web.Message);
                b.AddField("Exeption Response", web.Response);
                b.AddField("Exeption Status", web.Status);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "silver with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
        }

        [Command("stest", RunMode = RunMode.Async)]
        [RequireNsfw()]
        public async Task Nsfw()
        {
            await ReplyAsync("nsfw");
        }

        [Command("stest", RunMode = RunMode.Async)]
        public async Task Sfw()
        {
            await ReplyAsync("sfw");
        }

        [Command("meme", RunMode = RunMode.Async)]
        public async Task Kindsffeefdfdfergergrgfdfdsgfdfg()
        {
            EmbedBuilder b = new EmbedBuilder();
            HttpClient client = Webclient.Get();
            HttpResponseMessage rm = await client.GetAsync("https://meme-api.herokuapp.com/gimme");
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                meme asdf = System.Text.Json.JsonSerializer.Deserialize<meme>(await rm.Content.ReadAsStringAsync());
                if (!asdf.nsfw)
                {
                    SilverCraftBot.Modules.Language lang = Commands.GetLanguage(Context.Guild.Id);
                    b.WithTitle("meme: " + asdf.title);
                    b.WithUrl(asdf.postLink);
                    b.WithAuthor("👍 " + asdf.ups + " | r/" + asdf.subreddit);
                    b.WithFooter(lang.Requested_by + Context.User.Username, Commands.GetUserAvatarUrl(Context.User));
                    b.AddField("NSFW", asdf.nsfw, true);
                    b.AddField("Spoiler", asdf.spoiler, true);
                    b.AddField("Author", asdf.author, true);
                    b.WithImageUrl(asdf.url);
                    await ReplyAsync(embed: b.Build());
                }
                else
                {
                    SilverCraftBot.Modules.Language lang = Commands.GetLanguage(Context.Guild.Id);
                    b.WithTitle(lang.Meme_is_nsfw);

                    await ReplyAsync(embed: b.Build());
                }
            }
            else
            {
                b.WithDescription(rm.StatusCode.ToString());
                await ReplyAsync(embed: b.Build());
            }
        }

        private static MemoryStream Filter(byte[] photoBytes, IMatrixFilter e)
        {
            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            using MemoryStream inStream = new MemoryStream(photoBytes);
            MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Filter(e)
                            .Format(format)
                            .Save(outStream);
            }
            return outStream;
        }

        [Command("comic", RunMode = RunMode.Async)]
        public async Task Comic()
        {
            System.Collections.Generic.IReadOnlyCollection<Attachment> attachments = Context.Message.Attachments;
            if (attachments.Count == 0)
            {
                await Send_img_plsAsync("You didnt attach a image").ConfigureAwait(false);
                return;
            }
            else if (attachments.Count > 1)
            {
                await Send_img_plsAsync("You attached more than one image").ConfigureAwait(false);
                return;
            }
            await Context.Channel.SendFileAsync(Filter(new WebClient().DownloadData(attachments.ElementAt(0).Url), MatrixFilters.Comic), "silverbotimage.png", "There ya go a image with the comic filter");
        }

        [Command("comic", RunMode = RunMode.Async)]
        public async Task Comic(string urmom)
        {
            try
            {
                await Context.Channel.SendFileAsync(Filter(new WebClient().DownloadData(urmom), MatrixFilters.Comic), "silverbotimage.png", "There ya go a silver image");
            }
            catch (ArgumentNullException nullers)
            {
                SentrySdk.CaptureException(nullers);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(Convert.ToUInt64(dconfig.Log_channel)) as SocketTextChannel;
                b.WithTitle("Error at executing tint");
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + nullers);
                b.AddField("Exception Message", nullers.Message);
                b.AddField("Exeption ParamaterName", nullers.ParamName);
                b.AddField("Exeption InnerExeption", nullers.InnerException);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "silver with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
            catch (WebException web)
            {
                SentrySdk.CaptureException(web);
                EmbedBuilder b = new EmbedBuilder();
                SocketTextChannel channel = Program.GetClient().GetChannel(Convert.ToUInt64(dconfig.Log_channel)) as SocketTextChannel;
                b.WithTitle("Error at executing tint");
                Console.WriteLine(web);
                b.WithDescription("More detailed explination below\nError at command jpeg:\n" + web);
                b.AddField("Exception Message", web.Message);
                b.AddField("Exeption Response", web.Response);
                b.AddField("Exeption Status", web.Status);
                b.AddField("Message content", Context.Message.Content);
                b.AddField("Message url", Context.Message.GetJumpUrl());
                b.AddField("Message channel", Context.Channel.Id + "/" + Context.Channel.Name);
                b.AddField("Command", "silver with string");
                b.WithFooter("Requested by CONSOLE", "https://cdn.discordapp.com/attachments/728360861483401240/728362412373180566/console.png");
                await channel.SendMessageAsync("", false, b.Build());
                b = new EmbedBuilder();
                b.WithTitle("Error");
                b.WithDescription("Complete error sent to Bot logs");
                await ReplyAsync(embed: b.Build());
            }
        }

        private static Image DrawText(string text, Font font, System.Drawing.Color textColor, System.Drawing.Color backColor)
        {
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            SizeF textSize = drawing.MeasureString(text, font);
            img.Dispose();
            drawing.Dispose();
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);
            drawing = Graphics.FromImage(img);
            drawing.Clear(backColor);
            Brush textBrush = new SolidBrush(textColor);
            drawing.DrawString(text, font, textBrush, 0, 0);
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            return img;
        }

        [Command("text", RunMode = RunMode.Async)]
        public async Task Text(string e)
        {
            System.Drawing.Image img = DrawText(e, new Font("Diavlo Light", 30.0f), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(255, 255, 255));
            using MemoryStream outStream = new MemoryStream();
            img.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);
            outStream.Position = 0;
            await Context.Channel.SendFileAsync(outStream, "silverbotimage.png", "there");
        }

        [Command("usertest", RunMode = RunMode.Async)]
        public async Task Usertest()
        {
            Image img = DrawText(Context.User.Username + "#" + Context.User.DiscriminatorValue, new Font("Diavlo Light", 30.0f), System.Drawing.Color.FromArgb(0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0));
            using WebClient myWebClient = new WebClient();
            string url = Commands.GetUserAvatarUrl(Context.User);
            if (Commands.GetUserAvatarUrl(Context.User) == null)
            {
                url = Context.User.GetDefaultAvatarUrl();
            }
            byte[] photoBytes = myWebClient.DownloadData(url);
            ISupportedImageFormat format = new PngFormat { Quality = 70 };
            Size size = new Size(200, 200);
            using MemoryStream inStream = new MemoryStream(photoBytes);
            using MemoryStream avatarStream = new MemoryStream();
            using MemoryStream outStream = new MemoryStream();
            using (ImageFactory imageFactory = new ImageFactory())
            {
                imageFactory.Load(inStream)
                            .Resize(size)
                            .Format(format)
                            .Save(avatarStream);
            }
            avatarStream.Position = 0;
            Image imanidiot = Image.FromStream(avatarStream);

            Image imge = new Bitmap(800, 240);
            using (Graphics gr = Graphics.FromImage(imge))
            {
                gr.Clear(System.Drawing.Color.White);
                gr.DrawImage(imanidiot, new Point(13, 20));
                gr.DrawImage(img, new Point(229, 25));
            }
            imge.Save(outStream, System.Drawing.Imaging.ImageFormat.Png);

            outStream.Position = 0;
            await Context.Channel.SendFileAsync(outStream, "silverbotimage.png", "there");
        }
    }
}