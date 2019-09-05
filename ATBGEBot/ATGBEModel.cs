
public class Rootobject
{
    public string kind { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public string modhash { get; set; }
    public int dist { get; set; }
    public Child[] children { get; set; }
    public string after { get; set; }
    public object before { get; set; }
}

public class Child
{
    public string kind { get; set; }
    public Data1 data { get; set; }
}

public class Data1
{
    public object approved_at_utc { get; set; }
    public string subreddit { get; set; }
    public string selftext { get; set; }
    public string author_fullname { get; set; }
    public bool saved { get; set; }
    public object mod_reason_title { get; set; }
    public int gilded { get; set; }
    public bool clicked { get; set; }
    public string title { get; set; }
    public object[] link_flair_richtext { get; set; }
    public string subreddit_name_prefixed { get; set; }
    public bool hidden { get; set; }
    public int pwls { get; set; }
    public string link_flair_css_class { get; set; }
    public int downs { get; set; }
    public int thumbnail_height { get; set; }
    public bool hide_score { get; set; }
    public string name { get; set; }
    public bool quarantine { get; set; }
    public string link_flair_text_color { get; set; }
    public object author_flair_background_color { get; set; }
    public string subreddit_type { get; set; }
    public int ups { get; set; }
    public int total_awards_received { get; set; }
    public Media_Embed media_embed { get; set; }
    public int thumbnail_width { get; set; }
    public object author_flair_template_id { get; set; }
    public bool is_original_content { get; set; }
    public object[] user_reports { get; set; }
    public object secure_media { get; set; }
    public bool is_reddit_media_domain { get; set; }
    public bool is_meta { get; set; }
    public object category { get; set; }
    public Secure_Media_Embed secure_media_embed { get; set; }
    public string link_flair_text { get; set; }
    public bool can_mod_post { get; set; }
    public int score { get; set; }
    public object approved_by { get; set; }
    public string thumbnail { get; set; }
    public bool edited { get; set; }
    public object author_flair_css_class { get; set; }
    public object[] steward_reports { get; set; }
    public object[] author_flair_richtext { get; set; }
    public Gildings gildings { get; set; }
    public string post_hint { get; set; }
    public object content_categories { get; set; }
    public bool is_self { get; set; }
    public object mod_note { get; set; }
    public float created { get; set; }
    public string link_flair_type { get; set; }
    public int wls { get; set; }
    public object banned_by { get; set; }
    public string author_flair_type { get; set; }
    public string domain { get; set; }
    public bool allow_live_comments { get; set; }
    public object selftext_html { get; set; }
    public object likes { get; set; }
    public string suggested_sort { get; set; }
    public object banned_at_utc { get; set; }
    public object view_count { get; set; }
    public bool archived { get; set; }
    public bool no_follow { get; set; }
    public bool is_crosspostable { get; set; }
    public bool pinned { get; set; }
    public bool over_18 { get; set; }
    public Preview preview { get; set; }
    public object[] all_awardings { get; set; }
    public bool media_only { get; set; }
    public string link_flair_template_id { get; set; }
    public bool can_gild { get; set; }
    public bool spoiler { get; set; }
    public bool locked { get; set; }
    public object author_flair_text { get; set; }
    public bool visited { get; set; }
    public object num_reports { get; set; }
    public object distinguished { get; set; }
    public string subreddit_id { get; set; }
    public object mod_reason_by { get; set; }
    public object removal_reason { get; set; }
    public string link_flair_background_color { get; set; }
    public string id { get; set; }
    public bool is_robot_indexable { get; set; }
    public object report_reasons { get; set; }
    public string author { get; set; }
    public int num_crossposts { get; set; }
    public int num_comments { get; set; }
    public bool send_replies { get; set; }
    public string whitelist_status { get; set; }
    public bool contest_mode { get; set; }
    public object[] mod_reports { get; set; }
    public bool author_patreon_flair { get; set; }
    public object author_flair_text_color { get; set; }
    public string permalink { get; set; }
    public string parent_whitelist_status { get; set; }
    public bool stickied { get; set; }
    public string url { get; set; }
    public int subreddit_subscribers { get; set; }
    public float created_utc { get; set; }
    public object discussion_type { get; set; }
    public object media { get; set; }
    public bool is_video { get; set; }
    public Crosspost_Parent_List[] crosspost_parent_list { get; set; }
    public string crosspost_parent { get; set; }
}

public class Media_Embed
{
}

public class Secure_Media_Embed
{
}

public class Gildings
{
}

public class Preview
{
    public Image[] images { get; set; }
    public bool enabled { get; set; }
}

public class Image
{
    public Source source { get; set; }
    public Resolution2[] resolutions { get; set; }
    public Variants variants { get; set; }
    public string id { get; set; }
}

public class Source
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Variants
{
    public Obfuscated obfuscated { get; set; }
    public Nsfw nsfw { get; set; }
}

public class Obfuscated
{
    public Source1 source { get; set; }
    public Resolution[] resolutions { get; set; }
}

public class Source1
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Resolution
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Nsfw
{
    public Source2 source { get; set; }
    public Resolution1[] resolutions { get; set; }
}

public class Source2
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Resolution1
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Resolution2
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Crosspost_Parent_List
{
    public object approved_at_utc { get; set; }
    public string subreddit { get; set; }
    public string selftext { get; set; }
    public string author_fullname { get; set; }
    public bool saved { get; set; }
    public object mod_reason_title { get; set; }
    public int gilded { get; set; }
    public bool clicked { get; set; }
    public string title { get; set; }
    public object[] link_flair_richtext { get; set; }
    public string subreddit_name_prefixed { get; set; }
    public bool hidden { get; set; }
    public int pwls { get; set; }
    public object link_flair_css_class { get; set; }
    public int downs { get; set; }
    public int thumbnail_height { get; set; }
    public bool hide_score { get; set; }
    public string name { get; set; }
    public bool quarantine { get; set; }
    public string link_flair_text_color { get; set; }
    public object author_flair_background_color { get; set; }
    public string subreddit_type { get; set; }
    public int ups { get; set; }
    public int total_awards_received { get; set; }
    public Media_Embed1 media_embed { get; set; }
    public int thumbnail_width { get; set; }
    public object author_flair_template_id { get; set; }
    public bool is_original_content { get; set; }
    public object[] user_reports { get; set; }
    public object secure_media { get; set; }
    public bool is_reddit_media_domain { get; set; }
    public bool is_meta { get; set; }
    public object category { get; set; }
    public Secure_Media_Embed1 secure_media_embed { get; set; }
    public object link_flair_text { get; set; }
    public bool can_mod_post { get; set; }
    public int score { get; set; }
    public object approved_by { get; set; }
    public string thumbnail { get; set; }
    public bool edited { get; set; }
    public object author_flair_css_class { get; set; }
    public object[] steward_reports { get; set; }
    public object[] author_flair_richtext { get; set; }
    public Gildings1 gildings { get; set; }
    public string post_hint { get; set; }
    public object content_categories { get; set; }
    public bool is_self { get; set; }
    public object mod_note { get; set; }
    public float created { get; set; }
    public string link_flair_type { get; set; }
    public int wls { get; set; }
    public object banned_by { get; set; }
    public string author_flair_type { get; set; }
    public string domain { get; set; }
    public bool allow_live_comments { get; set; }
    public object selftext_html { get; set; }
    public object likes { get; set; }
    public object suggested_sort { get; set; }
    public object banned_at_utc { get; set; }
    public object view_count { get; set; }
    public bool archived { get; set; }
    public bool no_follow { get; set; }
    public bool is_crosspostable { get; set; }
    public bool pinned { get; set; }
    public bool over_18 { get; set; }
    public Preview1 preview { get; set; }
    public object[] all_awardings { get; set; }
    public bool media_only { get; set; }
    public bool can_gild { get; set; }
    public bool spoiler { get; set; }
    public bool locked { get; set; }
    public object author_flair_text { get; set; }
    public bool visited { get; set; }
    public object num_reports { get; set; }
    public object distinguished { get; set; }
    public string subreddit_id { get; set; }
    public object mod_reason_by { get; set; }
    public object removal_reason { get; set; }
    public string link_flair_background_color { get; set; }
    public string id { get; set; }
    public bool is_robot_indexable { get; set; }
    public object report_reasons { get; set; }
    public string author { get; set; }
    public int num_crossposts { get; set; }
    public int num_comments { get; set; }
    public bool send_replies { get; set; }
    public string whitelist_status { get; set; }
    public bool contest_mode { get; set; }
    public object[] mod_reports { get; set; }
    public bool author_patreon_flair { get; set; }
    public object author_flair_text_color { get; set; }
    public string permalink { get; set; }
    public string parent_whitelist_status { get; set; }
    public bool stickied { get; set; }
    public string url { get; set; }
    public int subreddit_subscribers { get; set; }
    public float created_utc { get; set; }
    public object discussion_type { get; set; }
    public object media { get; set; }
    public bool is_video { get; set; }
}

public class Media_Embed1
{
}

public class Secure_Media_Embed1
{
}

public class Gildings1
{
}

public class Preview1
{
    public Image1[] images { get; set; }
    public bool enabled { get; set; }
}

public class Image1
{
    public Source3 source { get; set; }
    public Resolution3[] resolutions { get; set; }
    public Variants1 variants { get; set; }
    public string id { get; set; }
}

public class Source3
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Variants1
{
}

public class Resolution3
{
    public string url { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}
