using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.View.Menu;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using FirstRunSlider.Helpers;
using Object = Java.Lang.Object;

namespace FirstRunSlider
{
    [Activity(Label = "WelcomeActivity", MainLauncher = true)]
    public class WelcomeActivity : AppCompatActivity
    {
        private ViewPager viewPager;
        private MyViewPagerAdapter myViewPagerAdapter;
        private LinearLayout dotsLayout;
        private TextView[] dots;
        private static int[] layouts;
        private Button btnSkip, btnNext;
        private PreferencesHelper prefManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Create your application here

            // Checking for first time launch - before calling setContentView()
            prefManager = new PreferencesHelper(this);
            if (!prefManager.IsFirstTimeLaunch())
            {
                LaunchHomeScreen();
                Finish();
            }

            SetContentView(Resource.Layout.Welcome);
            // Making notification bar transparent
            if ((int) Build.VERSION.SdkInt >= 21)
            {
                Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden | (StatusBarVisibility) SystemUiFlags.Fullscreen |
                                                      (StatusBarVisibility) SystemUiFlags.LayoutStable;
            }

            viewPager = (ViewPager) FindViewById(Resource.Id.view_pager);
            dotsLayout = (LinearLayout) FindViewById(Resource.Id.layoutDots);
            btnSkip = (Button) FindViewById(Resource.Id.btn_skip);
            btnNext = (Button) FindViewById(Resource.Id.btn_next);

            // layouts of all welcome sliders
            // add few more layouts if you want
            layouts = new int[]
            {
                Resource.Layout.welcome_slide1,
                Resource.Layout.welcome_slide2,
                Resource.Layout.welcome_slide3,
                Resource.Layout.welcome_slide4

            };

            // adding bottom dots
            AddBottomDots(0);
            // making notification bar transparent
            ChangeStatusBarColor();

            myViewPagerAdapter = new MyViewPagerAdapter();
            viewPager.Adapter = myViewPagerAdapter;

            viewPager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) =>
            {
                AddBottomDots(e.Position);

                // changing the next button text 'NEXT' / 'GOT IT'
                if (e.Position == layouts.Length - 1)
                {
                    // last page. make button text to GOT IT
                    btnNext.Text = GetString(Resource.String.start);
                    btnSkip.Visibility = ViewStates.Gone;
                }
                else
                {
                    // still pages are left
                    btnNext.Text = GetString(Resource.String.next);
                    btnSkip.Visibility = ViewStates.Visible;
                }
            };
            

            btnSkip.Click += delegate
            {
                LaunchHomeScreen();
            };
            btnNext.Click += delegate
            {
                // checking for last page
                // if last page home screen will be launched
                int current = GetItem(+1);
                if (current < layouts.Length)
                {
                    // move to next screen
                    viewPager.SetCurrentItem(current, true);
                }
                else
                {
                    LaunchHomeScreen();
                }
            };
        }


        private void AddBottomDots(int currentPage)
        {
            dots = new TextView[layouts.Length];

            string[] colorsActive = Resources.GetStringArray(Resource.Array.array_dot_active);
            string[] colorsInactive = Resources.GetStringArray(Resource.Array.array_dot_inactive);

            dotsLayout.RemoveAllViews();
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i] = new TextView(this);
                dots[i].Text = Html.FromHtml("&#8226;").ToString();
                dots[i].SetTextSize(ComplexUnitType.Dip, 35);
                dots[i].SetTextColor(Color.ParseColor(colorsInactive[currentPage]));
                dotsLayout.AddView(dots[i]);
            }

            if (dots.Length > 0)
                dots[currentPage].SetTextColor(Color.ParseColor(colorsActive[currentPage]));
        }


        private int GetItem(int i)
        {
            return viewPager.CurrentItem + i;
        }

        private void LaunchHomeScreen()
        {
            prefManager.SetFirstTimeLaunch(false);
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }

        /**
        * Making notification bar transparent
        */
        private void ChangeStatusBarColor()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window window = Window;
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                window.SetStatusBarColor(Color.Transparent);
            }
        }


        /**
        * View pager adapter
        */
        public class MyViewPagerAdapter : PagerAdapter
        {
            private LayoutInflater layoutInflater;

            public MyViewPagerAdapter()
            {
            }

            public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
            {
                layoutInflater = (LayoutInflater) Application.Context.GetSystemService(Context.LayoutInflaterService);

                View view = layoutInflater.Inflate(layouts[position], container, false);
                container.AddView(view);

                return view;
            }

            public override bool IsViewFromObject(View view, Object @object)
            {
                return view == @object;
            }

            public override int Count => layouts.Length;

            public override void DestroyItem(ViewGroup container, int position, Object @object)
            {
                View view = (View) @object;
                container.RemoveView(view);
            }
        }
    }
}