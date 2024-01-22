using dotnet_docker;
using dotnet_docker.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.IdentityModel.Abstractions;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace api.Model
{
    public class BankContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<Attempt> Attempts { get; set; }

        public BankContext(DbContextOptions<BankContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "bambik",
                    Balance = 10000,
                    AccountNumber = "1234567890",
                    CardNumber = "pFXN6+fgCGnqEQmma5n6kEwDgerlIzdFP3dwgMWr4GG0ZdUB7ANaFjRz//rqWLOR/SUzGEhP7doMMLJfvrOM/qyfzhTOeXKUGFJaQ63hLF4EFAeztA7uNYEtZo/rbvlMQSlDT7LYMAUJILAhBaAT65aaxDBKX+Dje9SMBAy4LEE=",
                    DocumentNumber = "TrU5RYGtGlapkGChyVeudJ7JplM6F48NmZ5/e6pT84Qk6SQXdPdcnYRuexHtaIO7Xw3EOfSM6AH2nLaGI1smaxNhjSz3vJEao0WVvfqAoLcsLXA2qsnnll0OukuAxVgzZexza0rTEY1l9KhkCwGdl5B1NoshOyENUsX2dp8IZ+M="

                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "rower",
                    Balance = 5000,
                    AccountNumber = "0987654321",
                    CardNumber = "cUPLo8vSRH6ZK+yc8SslJGIgJebJFGalX9Uedf6jAJhu55rJWmFM1iSTaoI1+GqdZBi8pTwtahOb2r1Y31+rixYFMMLuF/s3r8HyB18kJ2m8Hs+M9BeGBh9cCuT9jILQ1plq4ZIg7hqVmJ/vvCRzDA0ajuH3/6G4FJROtpqMkXM=",
                    DocumentNumber = "cXA/OS7lBWF3BME5qwp9q5TjU5oe6WiXv8QA5FSNoP9kks5Y2/08MBQHbepy1cuTtZB/DxugM+W+3P9waPpOxI3v7R78Cmfa2r5vKJPodoW4PLjAA4VgdsfyXyD70OZ/h8S3icKCYWch9kFCG0nfXNWOC6n2y5K7okSg1m6d0C0="
                });

            modelBuilder.Entity<Transfer>().HasData(
                new Transfer
                {
                    Id = Guid.NewGuid(),
                    Title = "Za lody",
                    ReceiverName = "Pan Rower",
                    ReceiverAdress = "ul. Rewolwerowa 36",
                    SenderUsername = "bambik",
                    ReceiverUsername = "rower",
                    Amount = 150,
                    Time = 1705752395
                },
                new Transfer
                {
                    Id = Guid.NewGuid(),
                    Title = "Za batony",
                    ReceiverName = "Pan Bambik",
                    ReceiverAdress = "ul. Palmowa 71",
                    SenderUsername = "rower",
                    ReceiverUsername = "bambik",
                    Amount = 30,
                    Time = 1705759543
                }
                );



            modelBuilder.Entity<Password>().HasData(
                new Password
                {
                    Id = Guid.Parse("385d11c8-a4d2-4b2c-8d05-00ed055a6855"),
                    Variant = 8,
                    Username = "bambik",
                    Pattern = "_ _ X _ _ _ X _ _ X X X X X _ X _ _ X X X",
                    Hash = "$2a$11$RuBDZn6EH6bg2rcae9cSiu1HdN8iXGEKWZn.E6R5gtBnu2g8sC8Ae"
                },
                new Password
                {
                    Id = Guid.Parse("639e5dfd-8d18-490a-9e88-5b3757bf64f6"),
                    Variant = 9,
                    Username = "bambik",
                    Pattern = "_ X X X X X X X _ _ _ _ _ _ X _ X _ _ X X",
                    Hash = "$2a$11$vJeqyCSKgJlJatfY.crXouKm6HI0WdOylhC12tgz5K33ysEMTETK."
                },
                new Password
                {
                    Id = Guid.Parse("68b33428-6d58-49df-acb8-d92fbdae6899"),
                    Variant = 3,
                    Username = "bambik",
                    Pattern = "_ X _ X _ _ X _ X _ X _ X X _ X X _ X _ X",
                    Hash = "$2a$11$2ayWxNfdf13AjDf10Sr2Q.YcQyJgwsL/faYjd8gHqPRRsVa0h4pZy"
                },
                new Password
                {
                    Id = Guid.Parse("6e57f67b-090d-4009-aa08-4d2e27c05a5e"),
                    Variant = 2,
                    Username = "bambik",
                    Pattern = "_ _ X X X _ _ X X X _ X _ X _ X X _ _ X _",
                    Hash = "$2a$11$aSlBI3ELmiBc/Z6LTgNOsuIR3eEFlbiH94FZWtn2Ds2dUFNnfrOvG"
                },
                new Password
                {
                    Id = Guid.Parse("82142441-a246-4559-b413-c7fe43250be8"),
                    Variant = 5,
                    Username = "bambik",
                    Pattern = "_ X X _ X X _ _ X X _ _ _ _ X X X X _ _ X",
                    Hash = "$2a$11$qmD8Czpsmavd2rwQZ5R6iulhPQKcwoSgXygd1XW1ieftSJcd071SW"
                },
                new Password
                {
                    Id = Guid.Parse("8c95d6e1-2c3f-4cd0-a438-e7b17ffa2993"),
                    Variant = 7,
                    Username = "bambik",
                    Pattern = "X X _ _ _ X X _ _ X _ X X _ X _ X _ X _ X",
                    Hash = "$2a$11$iGfUZf1exrivxIO/S24EC.gGpOa0zyht3gqxXs068dYMerxEUawsu"
                },
                new Password
                {
                    Id = Guid.Parse("8d858353-4738-4f56-9f4e-bec0da4e9827"),
                    Variant = 4,
                    Username = "bambik",
                    Pattern = "_ _ X X _ _ X _ X X X _ _ X _ X _ X X X _",
                    Hash = "$2a$11$NgYouQKG7JWEINlfblNtYuuYedcmQVNi1kauJ0w5549OHw4sgNVd."
                },
                new Password
                {
                    Id = Guid.Parse("b42ee2c4-0b36-458b-83eb-24393c933abe"),
                    Variant = 6,
                    Username = "bambik",
                    Pattern = "_ _ X X X X X _ X _ X _ X _ _ X _ _ X X _",
                    Hash = "$2a$11$i5xoltIkl4C26.RkIroQSOz8zDoxNlttPFZyvbzL5LWhwjzyBRewW"
                },
                new Password
                {
                    Id = Guid.Parse("dc5a31de-e33d-4781-a899-0bca784f757a"),
                    Variant = 1,
                    Username = "bambik",
                    Pattern = "X _ _ X _ X X _ X _ X X _ _ _ X X X _ _ X",
                    Hash = "$2a$11$KnJ6crZlHxrLfxGtJ7oCN.KKOVpRY0O.DhW3ijrnXX5Lcgz9jjY4C"
                },
                new Password
                {
                    Id = Guid.Parse("e61750ed-6723-4b9f-a4cb-b7671f27a196"),
                    Variant = 10,
                    Username = "bambik",
                    Pattern = "X _ X X X X X _ _ _ X _ _ _ X X _ X _ _ X",
                    Hash = "$2a$11$4zHcm0uhMPkq5k6Zj8v16eKzUlgKLA5Bc3iHZEar9nlF0dsan51rS"
                },


                new Password
                {
                    Id = Guid.Parse("12a2031b-7c03-482e-ae50-f324ab12975f"),
                    Variant = 2,
                    Username = "rower",
                    Pattern = "X X _ _ X X _ _ _ _ X X _ X",
                    Hash = "$2a$11$ISQ63pRM1pZCsN2ktr/Lzecn8IcGCImbytnsrqAs8z5.s3TF07Z/S"
                },
                new Password
                {
                    Id = Guid.Parse("5a455b8e-115c-4d4e-9c11-2cf0ec2379d4"),
                    Variant = 9,
                    Username = "rower",
                    Pattern = "_ X X _ _ X _ X _ X X _ X _",
                    Hash = "$2a$11$QXL7XCbOLj9elDzMfHm.yeXPKbLQXJevXc7oyn2FOJ2goskrrPLRG"
                },
                new Password
                {
                    Id = Guid.Parse("5ca5dddf-7948-49f8-8baa-06fb6f66db3e"),
                    Variant = 4,
                    Username = "rower",
                    Pattern = "_ X _ X _ _ X X _ _ X _ X X",
                    Hash = "$2a$11$BP4zDTQT97GNniivL1dEOOnVaQMYhrxaXksjRlXdVaV8IXqzoQixK"
                },
                new Password
                {
                    Id = Guid.Parse("5cd1f589-e5b1-405c-9e55-d0a224d1545d"),
                    Variant = 1,
                    Username = "rower",
                    Pattern = "_ X _ _ _ _ X _ X _ X X X X",
                    Hash = "$2a$11$8vDyPYe8u4BJKTHf06gE6u1MdfS/Jebmv9kG.wyGgSU.qow3qGq5u"
                },
                new Password
                {
                    Id = Guid.Parse("6b6f6b5b-652c-4ce3-a80a-b1561bcb0d7a"),
                    Variant = 3,
                    Username = "rower",
                    Pattern = "_ X X _ X _ _ X X _ _ X X _",
                    Hash = "$2a$11$QWtSlfb9IY708f6xAJgQJ.2flgPIvNCd4bO5kJ8WbEabSFG6vzqlG"
                },
                new Password
                {
                    Id = Guid.Parse("72206588-a22a-4e6a-baac-d4e07da678f9"),
                    Variant = 7,
                    Username = "rower",
                    Pattern = "X _ X _ _ _ _ X X _ _ X X X",
                    Hash = "$2a$11$s37/TZqfjGkrUUFjRgggwec2saeAvgk51qrHtkphUOdumiD.6rc5u"
                },
                new Password
                {
                    Id = Guid.Parse("94f13f52-f2df-4a79-b8d1-67856c0ebc26"),
                    Variant = 5,
                    Username = "rower",
                    Pattern = "_ _ X X X X _ X _ _ _ _ X X",
                    Hash = "$2a$11$rKoJETHS71Zun7N9c9xJoOR5a6.G1RfZmRR/lTJb2OZzhKZ1T3kUK"
                },
                new Password
                {
                    Id = Guid.Parse("ad22e254-eff9-4408-9b69-587118d833d0"),
                    Variant = 8,
                    Username = "rower",
                    Pattern = "X _ _ X _ X _ _ _ X _ X X X",
                    Hash = "$2a$11$NX1lQHDCuJpR59cOwL86E.vZwan7bDC1KnMj3vlK01GXmx8BVb1gG"
                },
                new Password
                {
                    Id = Guid.Parse("d99994ba-291d-4a7b-ae15-f0a82100d2b8"),
                    Variant = 10,
                    Username = "rower",
                    Pattern = "X X _ _ X _ _ _ _ X X _ X X",
                    Hash = "$2a$11$SGi9uh/Zvu6eImn6s06XxeUwWzea4793rLRU/mZVtU96mZvDNcw.q"
                },
                new Password
                {
                    Id = Guid.Parse("eec91ec5-10c9-48cf-92f1-3c85b2f7cda2"),
                    Variant = 6,
                    Username = "rower",
                    Pattern = "_ _ X X _ X _ _ _ _ X X X X",
                    Hash = "$2a$11$HDKD3oQ22/vd/z1N4pvcyOhjqPhQ8.m1AnKt/VqA7rjctSbribXz."
                });


        }
    }
}
