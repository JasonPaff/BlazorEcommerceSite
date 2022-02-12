using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Server.Migrations
{
    public partial class ProductSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { 1, ", the series follows the adventures of Arthur Dent, a hapless Englishman, following the destruction of the Earth by the Vogons (a race of unpleasant and bureaucratic aliens) to make way for an intergalactic bypass. Dent's adventures intersect with several other characters: Ford Prefect (an alien and researcher for the eponymous guidebook who rescues Dent from Earth's destruction), Zaphod Beeblebrox (Ford's eccentric semi-cousin and the Galactic President who has stolen the Heart of Gold — a spacecraft equipped with Infinite Improbability Drive), the depressed robot Marvin the Paranoid Android, and Trillian (formerly known as Tricia McMillan) who is a woman Arthur once met at a party in Islington and who — thanks to Beeblebrox's intervention — is the only other human survivor of Earth's destruction", "https://upload.wikimedia.org/wikipedia/en/b/bd/H2G2_UK_front_cover.jpg", 9.99m, "The Hitchhikers Guide to the Galaxy" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { 2, "It has been seven hundred years since mankind colonized other planets. The powerful ruling class of humans has installed a rigid, color-based social hierarchy where the physically superior Golds at the top rule with an iron fist. Sixteen-year-old Darrow is a Red, a class of workers who toil beneath the surface of Mars mining helium-3 to terraform the planet and make it habitable. He and his wife Eo are captured after entering a forbidden area and are arrested. While she is publicly whipped for her crime, Eo sings a forbidden folk tune as a protest against their virtual enslavement. She is subsequently hanged on the orders of Mars' ArchGovernor Nero au Augustus. Darrow cuts down and buries his wife's body, a crime for which he is also hanged. However, Darrow awakes to find that he has been drugged and delivered into the hands of the Sons of Ares, a terrorist group of Reds who fight against the oppression of the. They have adopted the video of Eo's song and execution as a rallying vehicle for their cause. Darrow joins the Sons when he learns that Mars was already terraformed centuries before and that the Reds have been tricked into perpetual servitude and subjugation.", "https://upload.wikimedia.org/wikipedia/en/thumb/9/9b/Red_Rising_%282014%29.jpg/220px-Red_Rising_%282014%29.jpg", 8.99m, "Red Rising" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageUrl", "Price", "Title" },
                values: new object[] { 3, "Drizzt was born to the tenth noble House of Menzoberranzan, Daermon Na'shezbaernon (more commonly known as House Do'Urden). He was the son of Malice, the Do'Urden Matron Mother and her consort, Do'Urden weaponmaster (and sometime Patron) Zaknafein. As the third son, drow culture demanded that Drizzt be sacrificed to their goddess Lolth. However, the death of his older brother, and the first son, Nalfein, (incidentally, at the treacherous hand of the second son, Dinin) in the battle against House DeVir which raised Daermon Na'shezbaernon to the 9th ranked in the city during his birth, made him the second son and spared him.", "https://upload.wikimedia.org/wikipedia/en/f/f3/Homeland_%28D%26D_novel%29.jpg", 7.99m, "Homeland" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
