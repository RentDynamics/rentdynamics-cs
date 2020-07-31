set -o pipefail -e

export RD_NUGET_USER=${1:-${RD_NUGET_USER:?"Github user login required for publishing is not defined. Set up RD_NUGET_USER env var or provide it as script parameter: './publish.sh username password'"}}
export RD_NUGET_PASSWORD=${2:-${RD_NUGET_PASSWORD:?"Github personal access token required for publishing is not defined. Set up RD_NUGET_PASSWORD env var or provide it as script parameter: './publish.sh username password'"}}

DEBUG=true # Set this to 'true' for debugging purposes
if $DEBUG; then
  RELEASE=false
else
  RELEASE=true
fi

default_debug_version="0.0.0-debug"

$DEBUG && echo 'DEBUG MODE IS ENABLED'

function fail() {
  printf '%s\n' "$1" >&2 # Send message to stderr.
  exit "${2-1}"          # Return a code specified by $2 or 1 by default.
}

function fail_release() {
  if $DEBUG; then
    debug_postfix=" (ERROR SKIPPED IN DEBUG MODE)"
  else
    debug_postfix=""
  fi

  printf '%s%s\n' "$1" "$debug_postfix" >&2 # Send message to stderr.
  $RELEASE && exit "${2-1}"                 # Return a code specified by $2 or 1 by default.
  return 0
}

function confirm() {
  read -p "$1" -n 1 -r
  echo #move to a new line
  [[ $REPLY =~ ^[Yy]$ ]] || fail "Confirmation canceled by user"
}

function get_version() {
  current_tags=$(git tag --points-at HEAD)
  [ -n "$current_tags" ] || fail_release "Git tag used for version number is not specified. Please, create a git tag and rerun the publishing process."

  current_tags_count=$(echo "$current_tags" | wc -l)
  [ $current_tags_count -eq 1 ] || fail_release "Current commit marked with more than one tag. Please make sure that commit used for building has a single tag specified."

  if [ $DEBUG ] && [ -z "$current_tags" ]; then
    current_tags=$default_debug_version
  fi

  echo "$current_tags"
}

function validate_git_branch_is_updated_from_remote() {
  git fetch
  local_head=$(git rev-parse HEAD)
  remote_upstream_head=$(git rev-parse @{u})

  echo "Local head: $local_head"
  echo "Remote upstream head: $remote_upstream_head"

  [ "$local_head" == "$remote_upstream_head" ] || fail_release "Remote branch differs from you local branch. Try pushing local changes with 'git push' or updating local repo with 'git pull'."
}

function validate_git_branch() {
  branch_name=$(git rev-parse --abbrev-ref HEAD)
  [ $branch_name == "master" ] || fail_release "Current git branch is '$branch_name'. Please, use only 'master' branch to create releases or enter debug mode for debugging purposes."
}

function validate_repo_is_clean() {
  local_changes=$(git status --porcelain)
  [ -z "$local_changes" ] && return 0 # if local_changes is not defined, then return success

  printf "Repository has local changes:\n%s\n" "$local_changes"
  fail_release "Please clean-up you local repository before creating a nuget package or enter debug mode for debugging purposes."
}

function validate_version_has_debug_postfix() {
  if [ "$1" == "$default_debug_version" ]; then
    fail "Default DEBUG version '$1' cannot be used for debug publishing. Please, create a version tag."
  fi

  debug_postfix="debug"
  if [[ "$1" =~ $debug_postfix ]]; then
    echo "Version tag '$1' is suitable for debug publishing"
    return 0
  else
    fail "Version tag '$1' does not contain postfix: '$debug_postfix' required for debug publishing. Either leave DEBUG mode or add postfix for the version"
  fi
}

version=$(get_version)
echo "Nuget package version is going to be '$version'"

validate_git_branch
validate_repo_is_clean
validate_git_branch_is_updated_from_remote

rm -rf publish_output

dotnet restore
dotnet test
dotnet pack -c "Release" -o publish_output -property:Version=$version

$DEBUG && validate_version_has_debug_postfix $version

confirm "Do you want to publish the nuget packages ($version)? (y/n)"

dotnet nuget push publish_output/**/*.nupkg --source github
git push origin $version # push git tag with version to the remote repository
